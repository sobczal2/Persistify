using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.HostedServices.Attributes;
using Persistify.Server.Management.Abstractions;
using Persistify.Server.Management.Abstractions.Exceptions;
using Persistify.Server.Management.Types.Abstractions;
using Persistify.Server.Persistence.DataStructures.Abstractions;
using Persistify.Server.Persistence.DataStructures.Providers;
using Persistify.Server.Persistence.DataStructures.Trees;

namespace Persistify.Server.Management.Types.Number;

[StartupPriority(3)]
public class NumberManager : ITypeManager<NumberManagerQuery, NumberManagerHit>, IActOnStartup
{
    private readonly IStorageProviderManager _storageProviderManager;
    private readonly ITemplateManager _templateManager;
    private readonly ConcurrentDictionary<TemplateFieldIdentifier, IAsyncTree<NumberManagerRecord>> _trees;

    public NumberManager(
        IStorageProviderManager storageProviderManager,
        ITemplateManager templateManager
    )
    {
        _storageProviderManager = storageProviderManager;
        _templateManager = templateManager;
        _trees = new ConcurrentDictionary<TemplateFieldIdentifier, IAsyncTree<NumberManagerRecord>>();
    }

    public ValueTask<IEnumerable<NumberManagerHit>> SearchAsync(NumberManagerQuery query)
    {
        return new ValueTask<IEnumerable<NumberManagerHit>>(new List<NumberManagerHit>(0));
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
            var tree = new AvlAsyncTree<NumberManagerRecord>(storageProvider, new NumberManagerRecordComparer());
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
