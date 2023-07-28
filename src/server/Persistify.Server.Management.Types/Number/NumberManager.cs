using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.HostedServices.Attributes;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Management.Abstractions.Exceptions;
using Persistify.Server.Management.Abstractions.Types;
using Persistify.Server.Management.Types.Number.Queries;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.DataStructures.Abstractions;
using Persistify.Server.Persistence.DataStructures.Trees;

namespace Persistify.Server.Management.Types.Number;

[StartupPriority(3)]
public class NumberManager : ITypeManager<NumberManagerQuery, NumberManagerHit>, IActOnStartup
{
    private readonly ITemplateManager _templateManager;
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILinearRepositoryManager _linearRepositoryManager;
    private readonly DataStructuresSettings _dataStructuresSettings;
    private readonly ConcurrentDictionary<TemplateFieldIdentifier, IAsyncLookup<double, long>> _lookups;

    public NumberManager(
        ITemplateManager templateManager,
        IRepositoryManager repositoryManager,
        ILinearRepositoryManager linearRepositoryManager,
        IOptions<DataStructuresSettings> dataStructuresSettingsOptions
    )
    {
        _templateManager = templateManager;
        _repositoryManager = repositoryManager;
        _linearRepositoryManager = linearRepositoryManager;
        _dataStructuresSettings = dataStructuresSettingsOptions.Value;
        _lookups = new ConcurrentDictionary<TemplateFieldIdentifier, IAsyncLookup<double, long>>();
    }

    public ValueTask<List<NumberManagerHit>> SearchAsync(NumberManagerQuery query)
    {
        // if (!_trees.TryGetValue(query.TemplateFieldIdentifier, out var tree))
        // {
        //     throw new ManagerInternalException();
        // }
        //
        // // TODO: Evaluate inside *NumberManagerQuery classes
        // switch (query)
        // {
        //     case RangeNumberManagerQuery rangeNumberManagerQuery:
        //         // ReSharper disable once CompareOfFloatsByEqualityOperator
        //         if(rangeNumberManagerQuery.MinValue == rangeNumberManagerQuery.MaxValue)
        //         {
        //             var rangeRecord = await tree.GetAsync(new NumberManagerRecord(rangeNumberManagerQuery.MinValue, 0));
        //             if(rangeRecord == null)
        //             {
        //                 return new List<NumberManagerHit>(0);
        //             }
        //             var rangeResult = new List<NumberManagerHit>(rangeRecord.DocumentIds.Count);
        //             foreach (var documentId in rangeRecord.DocumentIds)
        //             {
        //                 rangeResult.Add(new NumberManagerHit(documentId));
        //             }
        //             return rangeResult;
        //         }
        //         if(rangeNumberManagerQuery.MinValue > rangeNumberManagerQuery.MaxValue)
        //         {
        //             var rangeRecords2 = await tree.GetRangeAsync(double.MinValue, rangeNumberManagerQuery.MaxValue);
        //             rangeRecords2.AddRange(await tree.GetRangeAsync(rangeNumberManagerQuery.MinValue, double.MaxValue));
        //             return GetHits(rangeRecords2);
        //         }
        //         return GetHits(await tree.GetRangeAsync(rangeNumberManagerQuery.MinValue,
        //             rangeNumberManagerQuery.MaxValue));
        //     case EqualNumberManagerQuery equalNumberManagerQuery:
        //         var record = await tree.GetAsync(new NumberManagerRecord(equalNumberManagerQuery.Value, 0));
        //         if(record == null)
        //         {
        //             return new List<NumberManagerHit>(0);
        //         }
        //         var result = new List<NumberManagerHit>(record.DocumentIds.Count);
        //         foreach (var documentId in record.DocumentIds)
        //         {
        //             result.Add(new NumberManagerHit(documentId));
        //         }
        //         return result;
        //     case NotEqualNumberManagerQuery notEqualNumberManagerQuery:
        //         // TODO: Optimize
        //         var records = await tree.GetRangeAsync(double.MinValue, notEqualNumberManagerQuery.Value);
        //         records.AddRange(await tree.GetRangeAsync(notEqualNumberManagerQuery.Value, double.MaxValue));
        //         return GetHits(records);
        //     case GreaterThanNumberManagerQuery greaterThanNumberManagerQuery:
        //         return GetHits(await tree.GetRangeAsync(greaterThanNumberManagerQuery.Value + double.Epsilon, double.MaxValue));
        //     case GreaterThanOrEqualNumberManagerQuery greaterThanOrEqualNumberManagerQuery:
        //         return GetHits(await tree.GetRangeAsync(greaterThanOrEqualNumberManagerQuery.Value, double.MaxValue));
        //     case LessThanNumberManagerQuery lessThanNumberManagerQuery:
        //         return GetHits(await tree.GetRangeAsync(double.MinValue, lessThanNumberManagerQuery.Value - double.Epsilon));
        //     case LessThanOrEqualNumberManagerQuery lessThanOrEqualNumberManagerQuery:
        //         return GetHits(await tree.GetRangeAsync(double.MinValue, lessThanOrEqualNumberManagerQuery.Value));
        //     default:
        //         throw new ManagerInternalException();
        // }

        throw new NotImplementedException();
    }

    // No lock needed - template is locked by the caller
    public async ValueTask IndexAsync(int templateId, Document document)
    {
        foreach (var numberFieldValue in document.NumberFieldValues)
        {
            var identifier = new TemplateFieldIdentifier(templateId, numberFieldValue.FieldName);

            if (!_lookups.TryGetValue(identifier, out var lookup))
            {
                throw new ManagerInternalException();
            }

            await lookup.AddAsync(numberFieldValue.Value, document.Id);
        }
    }

    public ValueTask DeleteAsync(int templateId, long documentId)
    {
        var template = _templateManager.Get(templateId);
        if (template == null)
        {
            throw new ManagerInternalException();
        }

        foreach (var numberField in template.NumberFields)
        {
            var identifier = new TemplateFieldIdentifier(templateId, numberField.Name);
            if (!_lookups.TryGetValue(identifier, out var tree))
            {
                throw new ManagerInternalException();
            }

            // await tree.PerformActionByPredicateAndMaybeRemoveAsync(
            //     (value, docId) => value.DocumentIds.Contains(docId),
            //     (value, docId) => value.DocumentIds.Remove(docId) && value.DocumentIds.Count == 0,
            //     documentId);
        }

        return ValueTask.CompletedTask;
    }

    public async ValueTask InitializeForTemplate(Template template)
    {
        foreach (var numberField in template.NumberFields)
        {
            _repositoryManager.Create<BTreeInternalNode<double>>(GetRepositoryName(template.Id, numberField.Name, "BTreeInternalNode"));
            _repositoryManager.Create<BTreeLeafNode<double, long>>(GetRepositoryName(template.Id, numberField.Name, "BTreeLeafNode"));
            _linearRepositoryManager.Create(GetRepositoryName(template.Id, numberField.Name, "BTreeLinear"));
            var internalNodeRepository = _repositoryManager.Get<BTreeInternalNode<double>>(GetRepositoryName(template.Id, numberField.Name, "BTreeInternalNode"));
            var leafNodeRepository = _repositoryManager.Get<BTreeLeafNode<double, long>>(GetRepositoryName(template.Id, numberField.Name, "BTreeLeafNode"));
            var linearRepository = _linearRepositoryManager.Get(GetRepositoryName(template.Id, numberField.Name, "BTreeLinear"));
            var tree = new BTreeAsyncLookup<double, long>(internalNodeRepository, leafNodeRepository,
                linearRepository, _dataStructuresSettings.BTreeDegree, Comparer<double>.Default);
            await tree.InitializeAsync();
            var identifier = new TemplateFieldIdentifier(template.Id, numberField.Name);
            _lookups.TryAdd(identifier, tree);
        }
    }

    public ValueTask RemoveForTemplate(Template template)
    {
        foreach (var numberField in template.NumberFields)
        {
            _repositoryManager.Delete<BTreeInternalNode<double>>(GetRepositoryName(template.Id, numberField.Name, "BTreeInternalNode"));
            _repositoryManager.Delete<BTreeLeafNode<double, long>>(GetRepositoryName(template.Id, numberField.Name, "BTreeLeafNode"));
            _linearRepositoryManager.Delete(GetRepositoryName(template.Id, numberField.Name, "BTreeLinear"));
            var identifier = new TemplateFieldIdentifier(template.Id, numberField.Name);
            _lookups.TryRemove(identifier, out _);
        }

        return ValueTask.CompletedTask;
    }

    private string GetRepositoryName(int templateId, string fieldName, string subtype)
    {
        return $"Types/Number/{templateId}_{fieldName}.{subtype}";
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
