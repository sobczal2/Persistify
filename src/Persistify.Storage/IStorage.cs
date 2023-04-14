using OneOf;
using Persistify.Dtos.Common;
using Persistify.Storage.Common;

namespace Persistify.Storage;

public interface IStorage
{
    ValueTask<OneOf<StorageSuccess<EmptyDto>, StorageError>> SaveBlobAsync(string key, string data,
        bool overwrite = false, CancellationToken cancellationToken = default);

    ValueTask<OneOf<StorageSuccess<string>, StorageError>> LoadBlobAsync(string key,
        CancellationToken cancellationToken = default);
    ValueTask<OneOf<StorageSuccess<EmptyDto>, StorageError>> DeleteBlobAsync(string key, CancellationToken cancellationToken = default);
    ValueTask<OneOf<StorageSuccess<bool>, StorageError>> ExistsBlobAsync(string key, CancellationToken cancellationToken = default);
}