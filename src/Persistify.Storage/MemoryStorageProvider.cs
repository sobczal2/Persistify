using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Persistify.Storage;

public class MemoryStorageProvider : IStorageProvider
{
    private readonly ConcurrentDictionary<StorageProviderDirectory, ConcurrentDictionary<string, string>> _blobs;
    public MemoryStorageProvider()
    {
        _blobs = new ConcurrentDictionary<StorageProviderDirectory, ConcurrentDictionary<string, string>>();
        foreach (var directoryName in Enum.GetValues(typeof(StorageProviderDirectory)))
        {
            _blobs.TryAdd((StorageProviderDirectory) directoryName, new ConcurrentDictionary<string, string>());
        }
    }
    public Task<string> ReadBlob(StorageProviderDirectory directory, string blobName)
    {
        var blobDirectory = _blobs[directory];
        if(!blobDirectory.TryGetValue(blobName, out var blobContent))
            throw new Exception($"Blob {blobName} does not exist in directory {directory}");

        return Task.FromResult(blobContent);
    }

    public Task WriteBlob(StorageProviderDirectory directory, string blobName, string content)
    {
        var blobDirectory = _blobs[directory];
        if(!blobDirectory.TryAdd(blobName, content))
            throw new Exception($"Blob {blobName} already exists in directory {directory}");
        
        return Task.CompletedTask;
    }

    public Task DeleteBlob(StorageProviderDirectory directory, string blobName)
    {
        var blobDirectory = _blobs[directory];
        if(!blobDirectory.TryRemove(blobName, out _))
            throw new Exception($"Blob {blobName} does not exist in directory {directory}");
        
        return Task.CompletedTask;
    }

    public Task<bool> BlobExists(StorageProviderDirectory directory, string blobName)
    {
        var blobDirectory = _blobs[directory];
        return Task.FromResult(blobDirectory.ContainsKey(blobName));
    }

    public Task<string[]> ListBlobs(StorageProviderDirectory directory)
    {
        var blobDirectory = _blobs[directory];
        return Task.FromResult(blobDirectory.Keys.ToArray());
    }
}