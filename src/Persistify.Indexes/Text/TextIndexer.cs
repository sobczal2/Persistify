using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Persistify.DataStructures.Tries;
using Persistify.Indexes.Common;
using Persistify.Storage;
using Persistify.Tokens;
using Index = Persistify.Indexes.Common.Index;

namespace Persistify.Indexes.Text;

public class TextIndexer : IIndexer<string>, IPersisted
{
    private const string IndexerName = "textindexer";
    private ConcurrentDictionary<string, ITrie<Index>> _tries = default!;

    public Task IndexAsync(long id, Token<string> token, string typeName)
    {
        AssertInitialized();
        var trie = _tries.GetOrAdd(typeName, _ => new ConcurrentByteMapTrie<Index>());
        trie.Add(token.Value, new Index(id, token.Path));
        return Task.CompletedTask;
    }

    public Task IndexAsync(long id, IEnumerable<Token<string>> tokens, string typeName)
    {
        AssertInitialized();
        var trie = _tries.GetOrAdd(typeName, _ => new ConcurrentByteMapTrie<Index>());
        foreach (var token in tokens)
            trie.Add(token.Value, new Index(id, token.Path));

        return Task.CompletedTask;
    }

    public Task<IEnumerable<Index>> SearchAsync(ISearchPredicate searchPredicate, string typeName)
    {
        AssertInitialized();
        if (searchPredicate is not TextSearchPredicate textSearchPredicate)
            throw new ArgumentException("Search predicate must be of type TextSearchPredicate");

        var trie = _tries.GetOrAdd(typeName, _ => new ConcurrentByteMapTrie<Index>());
        return Task.FromResult(trie.Search(textSearchPredicate.Value));
    }

    public Task<int> RemoveAsync(long id, string typeName)
    {
        AssertInitialized();
        var trie = _tries.GetOrAdd(typeName, _ => new ConcurrentByteMapTrie<Index>());
        return Task.FromResult(trie.Remove(index => index.Id == id));
    }

    public async ValueTask LoadAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        var exists = await storage.ExistsBlobAsync(IndexerName, cancellationToken);
        _tries = new ConcurrentDictionary<string, ITrie<Index>>();
        if (!exists)
            return;
        var serializedTries = await storage.LoadBlobAsync(IndexerName, cancellationToken);
        var tries = JsonConvert.DeserializeObject<ConcurrentDictionary<string, ConcurrentByteMapTrie<Index>>>(serializedTries);
        if(tries == null) throw new InvalidOperationException("Could not deserialize tries");
        foreach (var (key, value) in tries)
            _tries.TryAdd(key, value);
    }

    public async ValueTask SaveAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        AssertInitialized();
        var serializedTries = JsonConvert.SerializeObject(_tries);
        await storage.SaveBlobAsync(IndexerName, serializedTries, cancellationToken);
    }

    private void AssertInitialized()
    {
        if (_tries == null)
            throw new InvalidOperationException("Indexer has not been initialized");
    }
}