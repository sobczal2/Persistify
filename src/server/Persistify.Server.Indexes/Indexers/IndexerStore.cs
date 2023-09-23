using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers;

public class IndexerStore
{
    private readonly ConcurrentDictionary<string, IIndexer> _indexers;

    public IndexerStore(
        Template template
    )
    {
        _indexers = new ConcurrentDictionary<string, IIndexer>();
        foreach (var field in template.TextFields)
        {
            _indexers.TryAdd(field.Name, new TextIndexer(field.Name));
        }

        foreach (var field in template.NumberFields)
        {
            _indexers.TryAdd(field.Name, new NumberIndexer(field.Name));
        }

        foreach (var field in template.BooleanFields)
        {
            _indexers.TryAdd(field.Name, new BoolIndexer(field.Name));
        }
    }

    public async ValueTask<List<ISearchResult>> SearchAsync(ISearchQuery query)
    {
        _indexers.TryGetValue(query.FieldName, out var indexer);
        if (indexer == null)
        {
            throw new InvalidOperationException($"Indexer for field {query.FieldName} not found");
        }

        return await indexer.SearchAsync(query);
    }

    public async ValueTask IndexAsync(Document document)
    {
        foreach (var indexer in _indexers.Values)
        {
            await indexer.IndexAsync(document);
        }
    }

    public async ValueTask DeleteAsync(Document document)
    {
        foreach (var indexer in _indexers.Values)
        {
            await indexer.DeleteAsync(document);
        }
    }
}
