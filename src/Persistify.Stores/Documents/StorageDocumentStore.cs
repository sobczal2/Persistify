using System.Threading;
using System.Threading.Tasks;
using OneOf;
using Persistify.Dtos.Common;
using Persistify.Storage;
using Persistify.Stores.Common;

namespace Persistify.Stores.Documents;

public class StorageDocumentStore : IDocumentStore
{
    private readonly IStorage _storage;
    private long _currentId;
    private const string _documentPrefix = "document_";

    public StorageDocumentStore(IStorage storage)
    {
        _storage = storage;
        _currentId = 0;
    }
    public async ValueTask<OneOf<StoreSuccess<string>, StoreError>> GetAsync(long documentId, CancellationToken cancellationToken = default)
    {
        if (documentId > _currentId)
            return new StoreError("Document with this id does not exist", StoreErrorType.NotFound);
        
        var exists = await _storage.ExistsBlobAsync(_documentPrefix + documentId, cancellationToken);
        if (exists.IsT1)
            return new StoreError(exists.AsT1.Message, StoreErrorType.NotFound);
        
        if (!exists.AsT0.Data)
            return new StoreError("Document with this id does not exist", StoreErrorType.NotFound);
        
        var result = await _storage.LoadBlobAsync(_documentPrefix + documentId, cancellationToken);
        if (result.IsT1)
            return new StoreError(result.AsT1.Message, StoreErrorType.StorageError);
        
        return new StoreSuccess<string>(result.AsT0.Data);
    }

    public async ValueTask<OneOf<StoreSuccess<long>, StoreError>> AddAsync(string document, CancellationToken cancellationToken = default)
    {
        await _storage.SaveBlobAsync(_documentPrefix + ++_currentId, document, cancellationToken: cancellationToken);
        return new StoreSuccess<long>(_currentId);
    }

    public async ValueTask<OneOf<StoreSuccess<EmptyDto>, StoreError>> RemoveAsync(long documentId, CancellationToken cancellationToken = default)
    {
        if (documentId > _currentId)
            return new StoreError("Document with this id does not exist", StoreErrorType.NotFound);
        
        var exists = await _storage.ExistsBlobAsync(_documentPrefix + documentId, cancellationToken);
        if (exists.IsT1)
            return new StoreError(exists.AsT1.Message, StoreErrorType.NotFound);
        
        if (!exists.AsT0.Data)
            return new StoreError("Document with this id does not exist", StoreErrorType.NotFound);
        
        var result = await _storage.DeleteBlobAsync(_documentPrefix + documentId, cancellationToken);
        if (result.IsT1)
            return new StoreError(result.AsT1.Message, StoreErrorType.StorageError);
        
        return new StoreSuccess<EmptyDto>(new EmptyDto());
    }

    public async ValueTask<OneOf<StoreSuccess<bool>, StoreError>> ExistsAsync(long documentId, CancellationToken cancellationToken = default)
    {
        if (documentId > _currentId)
            return new StoreSuccess<bool>(false);
        
        var exists = await _storage.ExistsBlobAsync(_documentPrefix + documentId, cancellationToken);
        if (exists.IsT1)
            return new StoreError(exists.AsT1.Message, StoreErrorType.NotFound);
        
        return new StoreSuccess<bool>(exists.AsT0.Data);
    }
}