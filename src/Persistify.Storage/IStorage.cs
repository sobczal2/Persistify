using OneOf;
using Persistify.Dtos.Common;
using Persistify.Storage.Common;

namespace Persistify.Storage;

public interface IStorage
{
    ValueTask<OneOf<StorageSuccess<EmptyDto>, StorageError>> SaveBlobAsync(string key, string data,
        bool overwrite = false);

    ValueTask<OneOf<StorageSuccess<string>, StorageError>> LoadBlobAsync(string key);
    ValueTask<OneOf<StorageSuccess<EmptyDto>, StorageError>> DeleteBlobAsync(string key);
    ValueTask<OneOf<StorageSuccess<bool>, StorageError>> ExistsBlobAsync(string key);
}