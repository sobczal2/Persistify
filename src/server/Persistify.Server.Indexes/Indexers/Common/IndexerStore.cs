using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Persistify.Domain.Documents;
using Persistify.Domain.Search.Queries;
using Persistify.Domain.Search.Queries.Aggregates;
using Persistify.Domain.Templates;
using Persistify.Helpers.Algorithms;
using Persistify.Server.ErrorHandling;
using Persistify.Server.Indexes.Indexers.Bool;
using Persistify.Server.Indexes.Indexers.Number;
using Persistify.Server.Indexes.Indexers.Text;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers.Common;

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

        foreach (var field in template.BoolFields)
        {
            _indexers.TryAdd(field.Name, new BoolIndexer(field.Name));
        }
    }

    public void Initialize(List<Document> documents)
    {
        foreach (var indexer in _indexers.Values)
        {
            indexer.Initialize(documents);
        }
    }

    public IEnumerable<ISearchResult> Search(SearchQuery query)
    {
        IEnumerable<ISearchResult>[]? results;
        switch (query)
        {
            case FieldSearchQuery fieldSearchQuery:
                if (_indexers.TryGetValue(fieldSearchQuery.GetFieldName(), out var indexer))
                {
                    return indexer.SearchAsync(query);
                }

                throw new PersistifyInternalException();
            case AndSearchQuery andSearchQuery:
                results = new IEnumerable<ISearchResult>[andSearchQuery.Queries.Count];

                for (var i = 0; i < results.Length; i++)
                {
                    results[i] = Search(andSearchQuery.Queries[i]);
                }

                return EnumerableHelpers.IntersectSorted(
                        Comparer<ISearchResult>.Create((a, b) => a.DocumentId.CompareTo(b.DocumentId)), results)
                    .ToList();
            case OrSearchQuery orSearchQuery:
                results = new IEnumerable<ISearchResult>[orSearchQuery.Queries.Count];

                for (var i = 0; i < results.Length; i++)
                {
                    results[i] = Search(orSearchQuery.Queries[i]);
                }

                return EnumerableHelpers.MergeSorted(
                        Comparer<ISearchResult>.Create((a, b) => a.DocumentId.CompareTo(b.DocumentId)), results)
                    .ToList();
        }

        throw new PersistifyInternalException();
    }

    public void Index(Document document)
    {
        foreach (var indexer in _indexers.Values)
        {
            indexer.IndexAsync(document);
        }
    }

    public void Delete(Document document)
    {
        foreach (var indexer in _indexers.Values)
        {
            indexer.DeleteAsync(document);
        }
    }
}
