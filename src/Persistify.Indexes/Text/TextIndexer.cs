using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Persistify.DataStructures.Tries;
using Persistify.Indexes.Common;
using Persistify.Storage;
using Persistify.Tokens;

namespace Persistify.Indexes.Text;

public class TextIndexer : IIndexer<string>, IPersisted
{
    private const string IndexerName = "textindexer";
    private ConcurrentDictionary<TypePath, ITrie<long>> _tries = default!;

    public Task IndexAsync(long id, Token<string> token, string typeName)
    {
        AssertInitialized();
        var trie = _tries.GetOrAdd(new TypePath(typeName, token.Path), _ => new ConcurrentByteMapTrie<long>());
        trie.Add(token.Value, id);
        return Task.CompletedTask;
    }

    public Task IndexAsync(long id, IEnumerable<Token<string>> tokens, string typeName)
    {
        AssertInitialized();
        var groupedTokens = tokens.GroupBy(token => token.Path);
        foreach (var tokenGroup in groupedTokens)
        {
            var trie = _tries.GetOrAdd(new TypePath(typeName, tokenGroup.Key), _ => new ConcurrentByteMapTrie<long>());
            foreach (var token in tokenGroup)
                trie.Add(token.Value, id);
        }

        return Task.CompletedTask;
    }

    public Task<IEnumerable<long>> SearchAsync(ISearchPredicate searchPredicate)
    {
        AssertInitialized();
        if (searchPredicate is not TextSearchPredicate textSearchPredicate)
            throw new ArgumentException("Search predicate must be of type TextSearchPredicate");

        var trie = _tries.GetOrAdd(new TypePath(searchPredicate.TypeName, searchPredicate.Path),
            _ => new ConcurrentByteMapTrie<long>());
        return Task.FromResult(trie.Search(textSearchPredicate.Value, textSearchPredicate.CaseSensitive,
            textSearchPredicate.Exact));
    }

    public Task<int> RemoveAsync(long id, string typeName)
    {
        AssertInitialized();
        var tries = _tries.Where(pair => pair.Key.TypeName == typeName);
        var count = tries.Sum(trie => trie.Value.Remove(index => index == id));

        return Task.FromResult(count);
    }

    public async ValueTask LoadAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        var exists = await storage.ExistsBlobAsync(IndexerName, cancellationToken);
        ConcurrentDictionary<TypePath, ITrie<long>> tries;
        if (!exists)
        {
            tries = new ConcurrentDictionary<TypePath, ITrie<long>>();
            _tries = tries;
            return;
        }

        var serializedTries = await storage.LoadBlobAsync(IndexerName, cancellationToken);
        var deserializedTries =
            JsonConvert.DeserializeObject<ConcurrentDictionary<TypePath, ConcurrentByteMapTrie<long>>>(serializedTries);
        if (deserializedTries == null) throw new InvalidOperationException("Could not deserialize tries");

        _tries = new ConcurrentDictionary<TypePath, ITrie<long>>();
        foreach (var trie in deserializedTries)
            _tries.TryAdd(trie.Key, trie.Value);
    }

    public async ValueTask SaveAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        AssertInitialized();

        ConcurrentDictionary<TypePath, ITrie<long>> tries;
        lock (_tries)
        {
            tries = new ConcurrentDictionary<TypePath, ITrie<long>>(_tries);
        }

        var serializedTries = JsonConvert.SerializeObject(tries);
        await storage.SaveBlobAsync(IndexerName, serializedTries, cancellationToken);
    }


    private void AssertInitialized()
    {
        if (_tries == null)
            throw new InvalidOperationException("Indexer has not been initialized");
    }
}