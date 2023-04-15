using System.Threading;
using System.Threading.Tasks;
using Persistify.Storage;

namespace Persistify.Stores.Documents;

public class StorageDocumentStore : IDocumentStore
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
        await _storage.SaveBlobAsync(DocumentPrefix + ++_currentId, document, cancellationToken);
        return _currentId;
    }

    public async ValueTask RemoveAsync(long documentId, CancellationToken cancellationToken = default)
    {
        await _storage.DeleteBlobAsync(DocumentPrefix + documentId, cancellationToken);
    }

    public async ValueTask<bool> ExistsAsync(long documentId, CancellationToken cancellationToken = default)
    {
        return await _storage.ExistsBlobAsync(DocumentPrefix + documentId, cancellationToken);
    }
}