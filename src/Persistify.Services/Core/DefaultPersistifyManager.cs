using System.Linq;
using System.Threading.Tasks;
using Persistify.Indexer.Index;
using Persistify.Indexer.Tokens;
using Persistify.Indexer.Types;
using Persistify.Storage;

namespace Persistify.Indexer.Core;

public class DefaultPersistifyManager : IPersistifyManager
{
    private readonly IIndexStore _indexStore;
    private readonly ITypeStore _typeStore;
    private readonly IStorageProvider _storageProvider;
    private readonly ITokenizer _tokenizer;

    public DefaultPersistifyManager(
        IIndexStore indexStore,
        ITypeStore typeStore,
        IStorageProvider storageProvider,
        ITokenizer tokenizer
        )
    {
        _indexStore = indexStore;
        _typeStore = typeStore;
        _storageProvider = storageProvider;
        _tokenizer = tokenizer;
    }
    public async Task<bool> InitTypeAsync(TypeDefinition typeDefinition)
    {
        return await _typeStore.InitTypeAsync(typeDefinition);
    }

    public async Task<bool> DropTypeAsync(string name)
    {
        _indexStore.Clear(name);
        return await _typeStore.DropTypeAsync(name);
    }

    public async Task<TypeDefinition[]> ListTypesAsync()
    {
        return await _typeStore.ListTypesAsync();
    }

    public async Task<long> IndexAsync(string type, string data)
    {
        var typeDefinition = await _typeStore.GetTypeAsync(type);
        var tokens = _tokenizer.Tokenize(typeDefinition, data);
        var id = _indexStore.Count(type) + 1;
        foreach (var token in tokens)
        {
            _indexStore.Add(type, token, id);
        }
        
        await _storageProvider.WriteBlob(StorageProviderDirectory.Documents, $"{type}_{id}", data);
        return id;
    }

    public async Task<Document[]> SearchAsync(string typeName, string query, int limit = 10, int offset = 0)
    {
        var ids = _indexStore.Search(typeName, query);
        ids = ids.Skip(offset).Take(limit).ToArray();
        var tasks = ids.Select(id => _storageProvider.ReadBlob(StorageProviderDirectory.Documents, $"{typeName}_{id}"));
        var documents = await Task.WhenAll(tasks);
        return documents.Select((document, index) => new Document
        {
            Id = ids[index],
            Type = typeName,
            Data = document
        }).ToArray();
    }

    public Task DeleteAsync(string type, long id)
    {
        throw new System.NotImplementedException();
    }
}