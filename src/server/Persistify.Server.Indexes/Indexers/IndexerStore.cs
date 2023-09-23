using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers;

public class IndexerStore : IIndexerStore
{
    private readonly IIndexerFactory _indexerFactory;
    private readonly ConcurrentDictionary<IndexerKey, IIndexer> _indexers;
    private bool _initialized;

    public IndexerStore(
        IIndexerFactory indexerFactory
        )
    {
        _indexerFactory = indexerFactory;
        _indexers = new ConcurrentDictionary<IndexerKey, IIndexer>();
    }

    public ValueTask InitializeAsync()
    {
        // TODO: Load indexers
        _initialized = true;
        return ValueTask.CompletedTask;
    }

    public async ValueTask<IEnumerable<ISearchResult>> Search(ISearchQuery query)
    {
        ThrowIfNotInitialized();
        _indexers.TryGetValue(query.IndexerKey, out var indexer);
        if (indexer == null)
        {
            throw new InvalidOperationException($"Indexer {query.IndexerKey} not found.");
        }

        return await indexer.SearchAsync(query);
    }

    public void Add(IndexerKey key)
    {
        ThrowIfNotInitialized();
        var result = _indexers.TryAdd(key, _indexerFactory.Create(key));
        if (!result)
        {
            throw new InvalidOperationException($"Indexer {key} already exists.");
        }
    }

    public void Remove(IndexerKey key)
    {
        ThrowIfNotInitialized();
        var result = _indexers.TryRemove(key, out var indexer);
        if (!result)
        {
            throw new InvalidOperationException($"Indexer {key} not found.");
        }
    }

    private void ThrowIfNotInitialized()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("Indexer store is not initialized.");
        }
    }
}
