using Persistify.Core.Indexes;
using Persistify.Core.Storage;
using Persistify.Core.Tokens;

namespace Persistify.Core.Handlers;

public class SearchHandler
{
    private readonly IndexesStore _indexesStore;
    private readonly IStorageProvider _storageProvider;
    private readonly ITokenizer _tokenizer;

    public SearchHandler(IndexesStore indexesStore, IStorageProvider storageProvider, ITokenizer tokenizer)
    {
        _indexesStore = indexesStore;
        _storageProvider = storageProvider;
        _tokenizer = tokenizer;
    }
    
    public async Task<(int, string)[]> Handle(string type, string query)
    {
        var indexesSet = _indexesStore.Get(type);
        var tokens = _tokenizer.Tokenize(query);
        var ids = new List<int>();
        foreach (var token in tokens)
        {
            var id = indexesSet.GetIds(token);
            ids.AddRange(id);
        }
        
        var records = new List<(int, string)>(ids.Count);
        foreach (var id in ids)
        {
            var record = await _storageProvider.GetRecord(type, id);
            if (record != null)
            {
                records.Add((id, record));
            }
        }

        return records.ToArray();
    }
}