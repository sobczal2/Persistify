using System.Linq;
using System.Threading.Tasks;
using Persistify.Storage;

namespace Persistify.Indexer.Types;

public class StorageProviderTypeStore : ITypeStore
{
    private readonly IStorageProvider _storageProvider;

    public StorageProviderTypeStore(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }
    public async Task<bool> InitTypeAsync(TypeDefinition typeDefinition)
    {
        var exists = await _storageProvider.BlobExists(StorageProviderDirectory.Types, typeDefinition.Name);
        if (exists)
        {
            return false;
        }
        
        await _storageProvider.WriteBlob(StorageProviderDirectory.Types, typeDefinition.Name, typeDefinition.Serialize());
        
        return true;
    }

    public async Task<TypeDefinition[]> ListTypesAsync()
    {
        var typeNames = await _storageProvider.ListBlobs(StorageProviderDirectory.Types);
        var typeDefinitions = await Task.WhenAll(typeNames.Select(async name =>
        {
            var typeDefinition = await _storageProvider.ReadBlob(StorageProviderDirectory.Types, name);
            return typeDefinition.Deserialize();
        }));
        
        return typeDefinitions;
    }

    public async Task<bool> DropTypeAsync(string typeName)
    {
        var exists = await _storageProvider.BlobExists(StorageProviderDirectory.Types, typeName);
        if (!exists)
        {
            return false;
        }
        
        await _storageProvider.DeleteBlob(StorageProviderDirectory.Types, typeName);
        
        return true;
    }

    public async Task<TypeDefinition> GetTypeAsync(string typeName)
    {
        var typeDefinition = await _storageProvider.ReadBlob(StorageProviderDirectory.Types, typeName);
        return typeDefinition.Deserialize();
    }
}