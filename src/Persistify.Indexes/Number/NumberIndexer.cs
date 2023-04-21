using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Persistify.DataStructures.Trees;
using Persistify.Indexes.Common;
using Persistify.Storage;
using Persistify.Tokens;
using Index = Persistify.Indexes.Common.Index;

namespace Persistify.Indexes.Number;

public class NumberIndexer : IIndexer<double>, IPersisted
{
    private const string IndexerName = "numberindexer";
    private ConcurrentDictionary<string, ITree<Index>> _trees = default!;
    public Task IndexAsync(long id, Token<double> token, string typeName)
    {
        AssertInitialized();
        var tree = _trees.GetOrAdd(typeName, _ => new ConcurrentIntervalTree<Index>());
        tree.Insert(new Index(id, token.Path), token.Value);
        return Task.CompletedTask;
    }

    public Task IndexAsync(long id, IEnumerable<Token<double>> tokens, string typeName)
    {
        AssertInitialized();
        var tree = _trees.GetOrAdd(typeName, _ => new ConcurrentIntervalTree<Index>());
        foreach (var token in tokens)
            tree.Insert(new Index(id, token.Path), token.Value);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<Index>> SearchAsync(ISearchPredicate searchPredicate, string typeName)
    {
        AssertInitialized();
        if (searchPredicate is not NumberSearchPredicate numberSearchPredicate)
            throw new ArgumentException("Search predicate must be of type NumberSearchPredicate");

        var tree = _trees.GetOrAdd(typeName, _ => new ConcurrentIntervalTree<Index>());
        return Task.FromResult(tree.Search(numberSearchPredicate.Min, numberSearchPredicate.Max));
    }

    public Task<int> RemoveAsync(long id, string typeName)
    {
        AssertInitialized();
        var tree = _trees.GetOrAdd(typeName, _ => new ConcurrentIntervalTree<Index>());
        return Task.FromResult(tree.Remove(index => index.Id == id));
    }

    public async ValueTask LoadAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        var exists = await storage.ExistsBlobAsync(IndexerName, cancellationToken);
        _trees = new ConcurrentDictionary<string, ITree<Index>>();
        if (!exists)
            return;
        var serializedTries = await storage.LoadBlobAsync(IndexerName, cancellationToken);
        var tries = JsonConvert.DeserializeObject<ConcurrentDictionary<string, ConcurrentIntervalTree<Index>>>(serializedTries);
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