using System.Collections.Concurrent;
using System.Collections.Generic;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Dtos.Documents.Search.Queries.Aggregates;
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

    public IndexerStore(
        Template template,
        IAnalyzerExecutorFactory analyzerExecutorFactory
    )
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
    }

    public IEnumerable<SearchResult> Search(SearchQueryDto queryDto)
    {
        return queryDto switch
        {
            FieldSearchQueryDto fieldSearchQueryDto => FieldSearch(fieldSearchQueryDto),
            AndSearchQueryDto andSearchQueryDto => AndSearch(andSearchQueryDto),
            OrSearchQueryDto orSearchQueryDto => OrSearch(orSearchQueryDto),
            _ => throw new InternalPersistifyException(message: "Invalid search query")
        };
    }

    private IEnumerable<SearchResult> FieldSearch(FieldSearchQueryDto queryDto)
    {
        if (_indexers.TryGetValue(queryDto.GetFieldName(), out var indexer))
        {
            return indexer.SearchAsync(queryDto);
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
            Comparer<SearchResult>.Create((a, b) => a.DocumentId.CompareTo(b.DocumentId)), (a, b) => a.Merge(b),
            results);
    }

    private IEnumerable<SearchResult> OrSearch(OrSearchQueryDto queryDto)
    {
        var results = new IEnumerable<SearchResult>[queryDto.SearchQueryDtos.Count];

        for (var i = 0; i < results.Length; i++)
        {
            results[i] = Search(queryDto.SearchQueryDtos[i]);
        }

        return EnumerableHelpers.MergeSorted(
            Comparer<SearchResult>.Create((a, b) => a.DocumentId.CompareTo(b.DocumentId)), (a, b) => a.Merge(b),
            results);
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
