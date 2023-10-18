using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Dtos.Documents.Search.Queries.Aggregates;
using Persistify.Helpers.Algorithms;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Indexes.Indexers.Bool;
using Persistify.Server.Indexes.Indexers.Number;
using Persistify.Server.Indexes.Indexers.Text;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers.Common;

public class IndexerStore
{
    private readonly ConcurrentDictionary<string, IIndexer> _indexers;

    public IndexerStore(
        Template template,
        IAnalyzerFactory analyzerFactory
    )
    {
        _indexers = new ConcurrentDictionary<string, IIndexer>();

        foreach (var field in template.Fields)
        {
            switch (field)
            {
                case BoolField boolField:
                    _indexers.TryAdd(field.Name, new BoolIndexer(field.Name));
                    break;
                case NumberField numberField:
                    _indexers.TryAdd(field.Name, new NumberIndexer(field.Name));
                    break;
                case TextField textField:
                    var analyzer = analyzerFactory.Create(textField.AnalyzerDescriptor);
                    _indexers.TryAdd(field.Name, new TextIndexer(field.Name, analyzer));
                    break;
                default:
                    throw new InternalPersistifyException();
            }
        }
    }

    public IEnumerable<SearchResult> Search(SearchQueryDto query)
    {
        return query switch
        {
            FieldSearchQueryDto fieldSearchQuery => FieldSearch(fieldSearchQuery),
            AndSearchQueryDto andSearchQuery => AndSearch(andSearchQuery),
            OrSearchQueryDto orSearchQuery => OrSearch(orSearchQuery),
            _ => throw new InternalPersistifyException(message: "Invalid search query")
        };
    }

    private IEnumerable<SearchResult> FieldSearch(FieldSearchQueryDto query)
    {
        if (_indexers.TryGetValue(query.GetFieldName(), out var indexer))
        {
            return indexer.SearchAsync(query);
        }

        throw new InternalPersistifyException();
    }

    private IEnumerable<SearchResult> AndSearch(AndSearchQueryDto query)
    {
        var results = new IEnumerable<SearchResult>[query.Queries.Count];

        for (var i = 0; i < results.Length; i++)
        {
            results[i] = Search(query.Queries[i]);
        }

        return EnumerableHelpers.IntersectSorted(
            Comparer<SearchResult>.Create((a, b) => a.DocumentId.CompareTo(b.DocumentId)), results);
    }

    private IEnumerable<SearchResult> OrSearch(OrSearchQueryDto query)
    {
        var results = new IEnumerable<SearchResult>[query.Queries.Count];

        for (var i = 0; i < results.Length; i++)
        {
            results[i] = Search(query.Queries[i]);
        }

        return EnumerableHelpers.MergeSorted(
            Comparer<SearchResult>.Create((a, b) => a.DocumentId.CompareTo(b.DocumentId)), results);
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
