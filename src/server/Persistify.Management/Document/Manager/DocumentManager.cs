using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Management.Document.Cache;
using Persistify.Persistance.Document;
using Persistify.Persistance.KeyValue;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Management.Document.Manager;

public class DocumentManager : IDocumentManager
{
    private const string DocumentIdKey = "DocumentId";
    private readonly IDocumentCache _documentCache;
    private readonly IDocumentStorage _documentStorage;
    private readonly IKeyValueStorage _keyValueStorage;
    private ConcurrentDictionary<string, long> _currentDocumentIds;

    public DocumentManager(
        IDocumentStorage documentStorage,
        IKeyValueStorage keyValueStorage,
        IDocumentCache documentCache,
        IOptions<HostedServicesSettings> hostedServicesSettingsOptions
    )
    {
        _documentStorage = documentStorage;
        _keyValueStorage = keyValueStorage;
        _documentCache = documentCache;
        _currentDocumentIds = new ConcurrentDictionary<string, long>();
        RecurrentActionInterval =
            TimeSpan.FromSeconds(hostedServicesSettingsOptions.Value.DocumentManagerIntervalSeconds);
    }

    public async ValueTask PerformShutdownActionAsync()
    {
        await SaveDocumentIdsAsync();
    }

    public async ValueTask PerformStartupActionAsync()
    {
        _currentDocumentIds.Clear();

        var documentIds = await _keyValueStorage.GetAsync<ConcurrentDictionary<string, long>>(DocumentIdKey);

        if (documentIds != null)
        {
            _currentDocumentIds = documentIds;
        }
    }

    public TimeSpan RecurrentActionInterval { get; }

    public async ValueTask PerformRecurrentActionAsync()
    {
        await SaveDocumentIdsAsync();
    }

    public async ValueTask<long> AddAsync(string templateName, Protos.Documents.Shared.Document document)
    {
        var documentId = _currentDocumentIds.AddOrUpdate(templateName, 1, (_, id) => id + 1);

        await _documentStorage.AddAsync(templateName, documentId, document);

        return documentId;
    }

    public async ValueTask<Protos.Documents.Shared.Document?> GetAsync(string templateName, long documentId)
    {
        var document = _documentCache.Get((templateName, documentId));

        if (document != null)
        {
            return document;
        }

        document = await _documentStorage.GetAsync(templateName, documentId);

        if (document != null)
        {
            _documentCache.Set((templateName, documentId), document);
        }

        return document;
    }

    public ValueTask DeleteAsync(string templateName, long documentId)
    {
        _documentCache.Remove((templateName, documentId));
        return _documentStorage.DeleteAsync(templateName, documentId);
    }

    private async ValueTask SaveDocumentIdsAsync()
    {
        await _keyValueStorage.SetAsync(DocumentIdKey, _currentDocumentIds);
    }
}
