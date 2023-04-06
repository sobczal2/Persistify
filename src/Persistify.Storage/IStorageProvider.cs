using System.Threading.Tasks;

namespace Persistify.Storage;

public interface IStorageProvider
{
    Task<string> ReadBlob(StorageProviderDirectory directory, string blobName);
    Task WriteBlob(StorageProviderDirectory directory, string blobName, string content);
    Task DeleteBlob(StorageProviderDirectory directory, string blobName);
    Task<bool> BlobExists(StorageProviderDirectory directory, string blobName);
    Task<string[]> ListBlobs(StorageProviderDirectory directory);
}