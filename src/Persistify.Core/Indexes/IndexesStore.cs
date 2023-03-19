using System.Collections.Concurrent;
using Persistify.Core.Storage;
using Persistify.Core.Tokens;

namespace Persistify.Core.Indexes;

public class IndexesStore
{
    private readonly IStorageProvider _storageProvider;
    private readonly ITokenizer _tokenizer;
    private ConcurrentDictionary<string, IndexesSet> _indexesSets;
    
    public IndexesStore(IStorageProvider storageProvider, ITokenizer tokenizer)
    {
        _storageProvider = storageProvider;
        _tokenizer = tokenizer;
        _indexesSets = new ConcurrentDictionary<string, IndexesSet>();
    }
    
    public async Task Initialize()
    {
        await _storageProvider.Initialize();
        var types = await _storageProvider.GetTypes();
        foreach (var type in types)
        {
            var indexesSet = new IndexesSet(_storageProvider, _tokenizer, type);
            await indexesSet.Initialize();
            _indexesSets.TryAdd(type, indexesSet);
        }
    }
    
    public IndexesSet Get(string type)
    {
        if (_indexesSets.TryGetValue(type, out var indexesSet))
        {
            return indexesSet;
        }
        
        indexesSet = new IndexesSet(_storageProvider, _tokenizer, type);
        _indexesSets.TryAdd(type, indexesSet);
        return indexesSet;
    }
}