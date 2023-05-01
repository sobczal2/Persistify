using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Persistify.Storage;

namespace Persistify.Stores.Documents;

public class StorageDocumentStore : IDocumentStore, IPersisted
{
    private const string DocumentPrefix = "document_";
    private readonly IStorage _storage;
    private long _currentId;

    public StorageDocumentStore(IStorage storage)
    {
        _storage = storage;
        _currentId = 0;
    }

    public async ValueTask<string> GetAsync(long documentId, CancellationToken cancellationToken = default)
    {
        return await _storage.LoadBlobAsync(DocumentPrefix + documentId, cancellationToken);
    }

    public async ValueTask<long> AddAsync(string document, CancellationToken cancellationToken = default)
    {
        var newId = Interlocked.Increment(ref _currentId);
        await _storage.SaveBlobAsync(DocumentPrefix + newId, document, cancellationToken);
        return newId;
    }

    public async ValueTask RemoveAsync(long documentId, CancellationToken cancellationToken = default)
    {
        await _storage.DeleteBlobAsync(DocumentPrefix + documentId, cancellationToken);
    }

    public async ValueTask<bool> ExistsAsync(long documentId, CancellationToken cancellationToken = default)
    {
        return await _storage.ExistsBlobAsync(DocumentPrefix + documentId, cancellationToken);
    }

    public async ValueTask LoadAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        var exists = await storage.ExistsBlobAsync("documents", cancellationToken);
        if (exists)
        {
            var result = await storage.LoadBlobAsync("documents", cancellationToken);
            _currentId = JsonConvert.DeserializeObject<long>(result);
        }
    }

    public ValueTask SaveAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        return storage.SaveBlobAsync("documents", JsonConvert.SerializeObject(_currentId), cancellationToken);
    }
}