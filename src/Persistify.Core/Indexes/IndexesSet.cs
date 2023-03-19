using System.Collections.Concurrent;
using Persistify.Core.Storage;
using Persistify.Core.Tokens;

namespace Persistify.Core.Indexes;

public class IndexesSet
{
    private readonly IStorageProvider _storageProvider;
    private readonly ITokenizer _tokenizer;
    private readonly string _type;
    private ConcurrentDictionary<string, LinkedList<int>> _indexes;
    
    public IndexesSet(IStorageProvider storageProvider, ITokenizer tokenizer,string type)
    {
        _storageProvider = storageProvider;
        _tokenizer = tokenizer;
        _type = type;
        _indexes = new ConcurrentDictionary<string, LinkedList<int>>();
    }
    
    public async Task Initialize()
    {
        var indexes = await _storageProvider.GetIndexes(_type);
        foreach (var (value, id) in indexes)
        {
            if (_indexes.TryGetValue(value, out var ids))
            {
                ids.AddLast(id);
            }
            else
            {
                ids = new LinkedList<int>();
                ids.AddLast(id);
                _indexes.TryAdd(value, ids);
            }
        }
    }

    public int[] GetIds(string token)
    {
        if (_indexes.TryGetValue(token, out var ids))
        {
            return ids.ToArray();
        }
        
        return new int[0];
    }
    
    public async Task<int> Index(string data)
    {
        var recordId = await _storageProvider.AddRecord(_type, data);
        var tokens = _tokenizer.Tokenize(data);
        foreach (var token in tokens)
        {
            await _storageProvider.AddIndex(_type, token, recordId);
            if (_indexes.TryGetValue(token, out var ids))
            {
                ids.AddLast(recordId);
            }
            else
            {
                ids = new LinkedList<int>();
                ids.AddLast(recordId);
                _indexes.TryAdd(token, ids);
            }
        }
        
        return recordId;
    }
}