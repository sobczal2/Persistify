using Persistify.Core.Indexes;
using Persistify.Core.Storage;
using Persistify.Core.Tokens;

namespace Persistify.Core.Handlers;

public class IndexHandler
{
    private readonly IndexesStore _indexesStore;
    private readonly IStorageProvider _storageProvider;
    private readonly ITokenizer _tokenizer;

    public IndexHandler(IndexesStore indexesStore, IStorageProvider storageProvider, ITokenizer tokenizer)
    {
        _indexesStore = indexesStore;
        _storageProvider = storageProvider;
        _tokenizer = tokenizer;
    }
    
    public async Task<int> Handle(string type, string data)
    {
        var indexesSet = _indexesStore.Get(type);
        return await indexesSet.Index(data);
    }
}