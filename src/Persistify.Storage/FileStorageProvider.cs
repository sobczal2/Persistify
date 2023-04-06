using System;
using System.Threading.Tasks;

namespace Persistify.Storage;

public class FileStorageProvider : IStorageProvider
{
    private readonly string _rootDirectory;
    public FileStorageProvider(string rootDirectory)
    {
        _rootDirectory = rootDirectory;
    }
    public async Task<string> ReadBlob(StorageProviderDirectory directory, string blobName)
    {
        EnsureDirectoryExists(directory);
        var blobPath = System.IO.Path.Combine(GetDirectoryPath(directory), blobName);
        var exists = System.IO.File.Exists(blobPath);
        if (!exists)
            throw new Exception($"Blob {blobName} does not exist in directory {directory}");
        
        var content = await System.IO.File.ReadAllTextAsync(blobPath);
        return content;
    }

    public async Task WriteBlob(StorageProviderDirectory directory, string blobName, string content)
    {
        EnsureDirectoryExists(directory);
        var blobPath = System.IO.Path.Combine(GetDirectoryPath(directory), blobName);
        var exists = System.IO.File.Exists(blobPath);
        if (exists)
            throw new Exception($"Blob {blobName} already exists in directory {directory}");
        
        await System.IO.File.WriteAllTextAsync(blobPath, content);
    }

    public Task DeleteBlob(StorageProviderDirectory directory, string blobName)
    {
        EnsureDirectoryExists(directory);
        var blobPath = System.IO.Path.Combine(GetDirectoryPath(directory), blobName);
        var exists = System.IO.File.Exists(blobPath);
        if (!exists)
            throw new Exception($"Blob {blobName} does not exist in directory {directory}");
        
        System.IO.File.Delete(blobPath);
        return Task.CompletedTask;
    }

    public Task<bool> BlobExists(StorageProviderDirectory directory, string blobName)
    {
        EnsureDirectoryExists(directory);
        var blobPath = System.IO.Path.Combine(GetDirectoryPath(directory), blobName);
        var exists = System.IO.File.Exists(blobPath);
        return Task.FromResult(exists);
    }

    public Task<string[]> ListBlobs(StorageProviderDirectory directory)
    {
        EnsureDirectoryExists(directory);
        var blobDirectory = GetDirectoryPath(directory);
        var blobs = System.IO.Directory.GetFiles(blobDirectory);
        return Task.FromResult(blobs);
    }
    
    private string GetDirectoryPath(StorageProviderDirectory directory)
    {
        return System.IO.Path.Combine(_rootDirectory, directory.ToString());
    }
    
    private void EnsureDirectoryExists(StorageProviderDirectory directory)
    {
        var directoryPath = GetDirectoryPath(directory);
        if (!System.IO.Directory.Exists(directoryPath))
            System.IO.Directory.CreateDirectory(directoryPath);
    }
}