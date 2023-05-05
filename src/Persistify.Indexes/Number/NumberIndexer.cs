using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Persistify.DataStructures.Trees;
using Persistify.Indexes.Common;
using Persistify.Storage;
using Persistify.Tokens;

namespace Persistify.Indexes.Number;

public class NumberIndexer : IIndexer<double>, IPersisted
{
    private const string IndexerName = "numberindexer";
    private ConcurrentDictionary<TypePath, ITree<long>> _trees = default!;
    public Task IndexAsync(long id, Token<double> token, string typeName)
    {
        AssertInitialized();
        var tree = _trees.GetOrAdd(new TypePath(typeName, token.Path), _ => new ConcurrentIntervalTree<long>());
        tree.Insert(id, token.Value);
        return Task.CompletedTask;
    }

    public Task IndexAsync(long id, IEnumerable<Token<double>> tokens, string typeName)
    {
        AssertInitialized();
        var groupedTokens = tokens.GroupBy(token => token.Path);
        foreach (var tokenGroup in groupedTokens)
        {
            var tree = _trees.GetOrAdd(new TypePath(typeName, tokenGroup.Key), _ => new ConcurrentIntervalTree<long>());
            foreach (var token in tokenGroup)
                tree.Insert(id, token.Value);
        }

        return Task.CompletedTask;
    }

    public Task<IEnumerable<long>> SearchAsync(ISearchPredicate searchPredicate)
    {
        AssertInitialized();
        if (searchPredicate is not NumberSearchPredicate numberSearchPredicate)
            throw new ArgumentException("Search predicate must be of type NumberSearchPredicate");

        var tree = _trees.GetOrAdd(new TypePath(searchPredicate.TypeName, searchPredicate.Path), _ => new ConcurrentIntervalTree<long>());
        return Task.FromResult(tree.Search(numberSearchPredicate.Min, numberSearchPredicate.Max));
    }

    public Task<int> RemoveAsync(long id, string typeName)
    {
        AssertInitialized();
        var trees = _trees.Where(pair => pair.Key.TypeName == typeName);
        var count = trees.Sum(trie => trie.Value.Remove(index => index == id));

        return Task.FromResult(count);
    }

    public async ValueTask LoadAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        var exists = await storage.ExistsBlobAsync(IndexerName, cancellationToken);
        _trees = new ConcurrentDictionary<TypePath, ITree<long>>();
        if (!exists)
            return;
        var serializedTries = await storage.LoadBlobAsync(IndexerName, cancellationToken);
        var tries = JsonConvert.DeserializeObject<ConcurrentDictionary<TypePath, ConcurrentIntervalTree<long>>>(serializedTries);
        if(tries == null) throw new InvalidOperationException("Could not deserialize tries");
        foreach (var (key, value) in tries)
            _trees.TryAdd(key, value);
    }

    public async ValueTask SaveAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        AssertInitialized();
        var serializedTries = JsonConvert.SerializeObject(_trees);
        await storage.SaveBlobAsync(IndexerName, serializedTries, cancellationToken);
    }
    
    private void AssertInitialized()
    {
        if (_trees == null)
            throw new InvalidOperationException("Indexer has not been initialized");
    }
}