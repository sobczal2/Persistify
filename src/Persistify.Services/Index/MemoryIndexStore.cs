using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Persistify.Common.Lifecycle;
using Persistify.DataStructures.Tries;
using Persistify.Indexer.Tokens;
using Persistify.Storage;

namespace Persistify.Indexer.Index;

public class MemoryIndexStore : IIndexStore, IPersistedService
{
    private readonly Func<ITrie<long>> _trieFactory;
    private readonly ConcurrentDictionary<string, ITrie<long>> _tries;

    public MemoryIndexStore(Func<ITrie<long>> trieFactory)
    {
        _trieFactory = trieFactory;
        _tries = new ConcurrentDictionary<string, ITrie<long>>();
    }
    public void Add(string typeName, Token token, long id)
    {
        var trie = _tries.GetOrAdd(typeName, _ => _trieFactory());
        trie.Add(token.Value, id);
    }

    public long[] Search(string typeName, string query)
    {
        if (!_tries.TryGetValue(typeName, out var trie))
            return Array.Empty<long>();

        return trie.Search(query).ToArray();
    }

    public bool Remove(string typeName, long id)
    {
        throw new System.NotImplementedException();
    }

    public void Clear(string typeName)
    {
        _tries.TryRemove(typeName, out _);
    }

    public long Count(string typeName)
    {
        _tries.TryGetValue(typeName, out var trie);
        return trie?.UniqueItemsCount ?? 0;
    }

    public async Task LoadAsync(IStorageProvider storageProvider)
    {
        var blobNames = await storageProvider.ListBlobs(StorageProviderDirectory.Indexes);
        var tasks = blobNames.Select(async (blobName) =>
        {
            var trieContent = await storageProvider.ReadBlob(StorageProviderDirectory.Indexes, blobName);
            var trie = JsonConvert.DeserializeObject<ITrie<long>>(trieContent);
            _tries.TryAdd(blobName, trie);
        });
        await Task.WhenAll(tasks);
    }

    public async Task SaveAsync(IStorageProvider storageProvider)
    {
        var tasks = _tries.Select(async (pair) =>
        {
            var trieContent = JsonConvert.SerializeObject(pair.Value);
            await storageProvider.WriteBlob(StorageProviderDirectory.Indexes, pair.Key, trieContent);
        });
        await Task.WhenAll(tasks);
    }
}