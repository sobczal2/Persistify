using System.Collections.Concurrent;
using System.Collections.Generic;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Dtos.Documents.Search.Queries.Aggregates;
using Persistify.Dtos.Documents.Search.Queries.Common;
using Persistify.Helpers.Collections;
using Persistify.Server.Domain.Documents;
using Persistify.Server.Domain.Templates;
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
    private readonly IIndexer _allIndexer;

    public IndexerStore(Template template, IAnalyzerExecutorFactory analyzerExecutorFactory)
    {
        _indexers = new ConcurrentDictionary<string, IIndexer>();

        foreach (var field in template.Fields)
        {
            switch (field)
            {
                case BoolField:
                    _indexers.TryAdd(field.Name, new BoolIndexer(field.Name));
                    break;
                case NumberField:
                    _indexers.TryAdd(field.Name, new NumberIndexer(field.Name));
                    break;
                case TextField textField:
                    var analyzer = analyzerExecutorFactory.Create(textField.Analyzer);
                    _indexers.TryAdd(field.Name, new TextIndexer(field.Name, analyzer));
                    break;
                default:
                    throw new InternalPersistifyException();
            }
        }

        _allIndexer = new AllIndexer();
    }

    public IEnumerable<SearchResult> Search(SearchQueryDto queryDto)
    {
        return queryDto switch
        {
            FieldSearchQueryDto fieldSearchQueryDto => FieldSearch(fieldSearchQueryDto),
            AndSearchQueryDto andSearchQueryDto => AndSearch(andSearchQueryDto),
            OrSearchQueryDto orSearchQueryDto => OrSearch(orSearchQueryDto),
            NotSearchQueryDto notSearchQueryDto
                => Negate(Search(notSearchQueryDto.SearchQueryDto), notSearchQueryDto.Boost),
            AllSearchQueryDto allSearchQueryDto => _allIndexer.Search(allSearchQueryDto),
            _ => throw new InternalPersistifyException(message: "Invalid search query")
        };
    }

    private IEnumerable<SearchResult> FieldSearch(FieldSearchQueryDto queryDto)
    {
        if (_indexers.TryGetValue(queryDto.GetFieldName(), out var indexer))
        {
            return indexer.Search(queryDto);
        }

        throw new InternalPersistifyException();
    }

    private IEnumerable<SearchResult> AndSearch(AndSearchQueryDto queryDto)
    {
        var results = new IEnumerable<SearchResult>[queryDto.SearchQueryDtos.Count];

        for (var i = 0; i < results.Length; i++)
        {
            results[i] = Search(queryDto.SearchQueryDtos[i]);
        }

        return EnumerableHelpers.IntersectSorted(
            Comparer<SearchResult>.Create((a, b) => a.DocumentId.CompareTo(b.DocumentId)),
            (a, b) => a.Merge(b),
            results
        );
    }

    private IEnumerable<SearchResult> OrSearch(OrSearchQueryDto queryDto)
    {
        var results = new IEnumerable<SearchResult>[queryDto.SearchQueryDtos.Count];

        for (var i = 0; i < results.Length; i++)
        {
            results[i] = Search(queryDto.SearchQueryDtos[i]);
        }

        return EnumerableHelpers.MergeSorted(
            Comparer<SearchResult>.Create((a, b) => a.DocumentId.CompareTo(b.DocumentId)),
            (a, b) => a.Merge(b),
            results
        );
    }

    private IEnumerable<SearchResult> Negate(IEnumerable<SearchResult> results, float boost)
    {
        var resultEnumerator = results.GetEnumerator();
        var allSearchResults = _allIndexer.Search(new AllSearchQueryDto() { Boost = boost });

        foreach (var allSearchResult in allSearchResults)
        {
            if (
                resultEnumerator.MoveNext()
                && resultEnumerator.Current.DocumentId == allSearchResult.DocumentId
            )
            {
                continue;
            }

            yield return allSearchResult;
        }
    }

    public void Index(Document document)
    {
        foreach (var indexer in _indexers.Values)
        {
            indexer.Index(document);
        }

        _allIndexer.Index(document);
    }

    public void Delete(Document document)
    {
        foreach (var indexer in _indexers.Values)
        {
            indexer.Delete(document);
        }

        _allIndexer.Delete(document);
    }
}
