using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.HostedServices.Attributes;
using Persistify.Server.Management.Abstractions;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Management.Abstractions.Exceptions;
using Persistify.Server.Management.Abstractions.Types;
using Persistify.Server.Management.Types.Number.Queries;
using Persistify.Server.Persistence.DataStructures.Abstractions;
using Persistify.Server.Persistence.DataStructures.Providers;
using Persistify.Server.Persistence.DataStructures.Trees;

namespace Persistify.Server.Management.Types.Number;

[StartupPriority(3)]
public class NumberManager : ITypeManager<NumberManagerQuery, NumberManagerHit>, IActOnStartup
{
    private readonly IStorageProviderManager _storageProviderManager;
    private readonly ITemplateManager _templateManager;
    private readonly ConcurrentDictionary<TemplateFieldIdentifier, IAsyncRangeTree<NumberManagerRecord>> _trees;

    public NumberManager(
        IStorageProviderManager storageProviderManager,
        ITemplateManager templateManager
    )
    {
        _storageProviderManager = storageProviderManager;
        _templateManager = templateManager;
        _trees = new ConcurrentDictionary<TemplateFieldIdentifier, IAsyncRangeTree<NumberManagerRecord>>();
    }

    public async ValueTask<List<NumberManagerHit>> SearchAsync(NumberManagerQuery query)
    {
        if (!_trees.TryGetValue(query.TemplateFieldIdentifier, out var tree))
        {
            throw new ManagerInternalException();
        }

        // TODO: Evaluate inside *NumberManagerQuery classes
        switch (query)
        {
            case RangeNumberManagerQuery rangeNumberManagerQuery:
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if(rangeNumberManagerQuery.MinValue == rangeNumberManagerQuery.MaxValue)
                {
                    var rangeRecord = await tree.GetAsync(new NumberManagerRecord(rangeNumberManagerQuery.MinValue, 0));
                    if(rangeRecord == null)
                    {
                        return new List<NumberManagerHit>(0);
                    }
                    var rangeResult = new List<NumberManagerHit>(rangeRecord.DocumentIds.Count);
                    foreach (var documentId in rangeRecord.DocumentIds)
                    {
                        rangeResult.Add(new NumberManagerHit(documentId));
                    }
                    return rangeResult;
                }
                if(rangeNumberManagerQuery.MinValue > rangeNumberManagerQuery.MaxValue)
                {
                    var rangeRecords2 = await tree.GetRangeAsync(double.MinValue, rangeNumberManagerQuery.MaxValue);
                    rangeRecords2.AddRange(await tree.GetRangeAsync(rangeNumberManagerQuery.MinValue, double.MaxValue));
                    return GetHits(rangeRecords2);
                }
                return GetHits(await tree.GetRangeAsync(rangeNumberManagerQuery.MinValue,
                    rangeNumberManagerQuery.MaxValue));
            case EqualNumberManagerQuery equalNumberManagerQuery:
                var record = await tree.GetAsync(new NumberManagerRecord(equalNumberManagerQuery.Value, 0));
                if(record == null)
                {
                    return new List<NumberManagerHit>(0);
                }
                var result = new List<NumberManagerHit>(record.DocumentIds.Count);
                foreach (var documentId in record.DocumentIds)
                {
                    result.Add(new NumberManagerHit(documentId));
                }
                return result;
            case NotEqualNumberManagerQuery notEqualNumberManagerQuery:
                // TODO: Optimize
                var records = await tree.GetRangeAsync(double.MinValue, notEqualNumberManagerQuery.Value);
                records.AddRange(await tree.GetRangeAsync(notEqualNumberManagerQuery.Value, double.MaxValue));
                return GetHits(records);
            case GreaterThanNumberManagerQuery greaterThanNumberManagerQuery:
                return GetHits(await tree.GetRangeAsync(greaterThanNumberManagerQuery.Value + double.Epsilon, double.MaxValue));
            case GreaterThanOrEqualNumberManagerQuery greaterThanOrEqualNumberManagerQuery:
                return GetHits(await tree.GetRangeAsync(greaterThanOrEqualNumberManagerQuery.Value, double.MaxValue));
            case LessThanNumberManagerQuery lessThanNumberManagerQuery:
                return GetHits(await tree.GetRangeAsync(double.MinValue, lessThanNumberManagerQuery.Value - double.Epsilon));
            case LessThanOrEqualNumberManagerQuery lessThanOrEqualNumberManagerQuery:
                return GetHits(await tree.GetRangeAsync(double.MinValue, lessThanOrEqualNumberManagerQuery.Value));
            default:
                throw new ManagerInternalException();
        }
    }

    private List<NumberManagerHit> GetHits(List<NumberManagerRecord> records)
    {
        var hits = new SortedSet<long>();
        foreach (var record in records)
        {
            hits.UnionWith(record.DocumentIds);
        }

        var result = new List<NumberManagerHit>(hits.Count);
        foreach (var hit in hits)
        {
            result.Add(new NumberManagerHit(hit));
        }

        return result;
    }

    // No lock needed - template is locked by the caller
    public async ValueTask IndexAsync(int templateId, Document document)
    {
        foreach (var numberFieldValue in document.NumberFieldValues)
        {
            var identifier = new TemplateFieldIdentifier(templateId, numberFieldValue.FieldName);
            var record = new NumberManagerRecord(numberFieldValue.Value, document.Id);

            if (!_trees.TryGetValue(identifier, out var tree))
            {
                throw new ManagerInternalException();
            }

            await tree.InsertOrPerformActionOnValueAsync(
                record,
                (value, docId) => value.DocumentIds.Add(docId),
                document.Id);
        }
    }

    public async ValueTask DeleteAsync(int templateId, long documentId)
    {
        var template = _templateManager.Get(templateId);
        if (template == null)
        {
            throw new ManagerInternalException();
        }

        foreach (var numberField in template.NumberFields)
        {
            var identifier = new TemplateFieldIdentifier(templateId, numberField.Name);
            if (!_trees.TryGetValue(identifier, out var tree))
            {
                throw new ManagerInternalException();
            }

            await tree.PerformActionByPredicateAndMaybeRemoveAsync(
                (value, docId) => value.DocumentIds.Contains(docId),
                (value, docId) => value.DocumentIds.Remove(docId) && value.DocumentIds.Count == 0,
                documentId);
        }
    }

    public ValueTask InitializeForTemplate(Template template)
    {
        foreach (var numberField in template.NumberFields)
        {
            _storageProviderManager.Create<AvlAsyncTree<NumberManagerRecord>.Node>(
                GetStorageProviderName(template.Id, numberField.Name));
            var storageProvider =
                _storageProviderManager.Get<AvlAsyncTree<NumberManagerRecord>.Node>(
                    GetStorageProviderName(template.Id, numberField.Name));
            var tree = new AvlAsyncRangeTree<NumberManagerRecord>(storageProvider, new NumberManagerRecordComparer());
            var identifier = new TemplateFieldIdentifier(template.Id, numberField.Name);
            _trees.TryAdd(identifier, tree);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveForTemplate(Template template)
    {
        foreach (var numberField in template.NumberFields)
        {
            _storageProviderManager.Delete<AvlAsyncTree<NumberManagerRecord>.Node>(
                GetStorageProviderName(template.Id, numberField.Name));
            var identifier = new TemplateFieldIdentifier(template.Id, numberField.Name);
            if (!_trees.TryRemove(identifier, out _))
            {
                throw new ManagerInternalException();
            }
        }

        return ValueTask.CompletedTask;
    }

    private string GetStorageProviderName(int templateId, string fieldName)
    {
        return $"Types/Number/{templateId}_{fieldName}";
    }

    public async ValueTask PerformStartupActionAsync()
    {
        var templates = _templateManager.GetAll();
        foreach (var template in templates)
        {
            await InitializeForTemplate(template);
        }
    }
}
