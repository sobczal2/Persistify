using System;
using System.Collections.Generic;
using Persistify.Domain.Documents;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Dtos.Documents.Search.Queries.Number;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Indexes.DataStructures.Trees;
using Persistify.Server.Indexes.Indexers.Common;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers.Number;

public class NumberIndexer : IIndexer
{
    private readonly IntervalTree<NumberIndexerIntervalTreeRecord> _intervalTree;

    public NumberIndexer(string fieldName)
    {
        FieldName = fieldName;
        _intervalTree = new IntervalTree<NumberIndexerIntervalTreeRecord>();
    }

    public string FieldName { get; }

    public void IndexAsync(Document document)
    {
        var numberFieldValue = document.GetNumberFieldValueByName(FieldName);
        if (numberFieldValue == null)
        {
            throw new InternalPersistifyException();
        }

        _intervalTree.Insert(new NumberIndexerIntervalTreeRecord
        {
            DocumentId = document.Id, Value = numberFieldValue.Value
        });
    }

    public IEnumerable<SearchResult> SearchAsync(SearchQueryDto query)
    {
        if (query is not NumberSearchQueryDto numberSearchQuery || numberSearchQuery.GetFieldName() != FieldName)
        {
            throw new Exception("Invalid search query");
        }

        return query switch
        {
            ExactNumberSearchQueryDto exactNumberSearchQuery => HandleExactNumberSearch(exactNumberSearchQuery),
            GreaterNumberSearchQueryDto greaterNumberSearchQuery => HandleGreaterNumberSearch(greaterNumberSearchQuery),
            LessNumberSearchQueryDto lessNumberSearchQuery => HandleLessNumberSearch(lessNumberSearchQuery),
            RangeNumberSearchQueryDto rangeNumberSearchQuery => HandleRangeNumberSearch(rangeNumberSearchQuery),
            _ => throw new InternalPersistifyException(message: "Invalid search query")
        };
    }

    public void DeleteAsync(Document document)
    {
        _intervalTree.Remove(x => x.DocumentId == document.Id);
    }

    private IEnumerable<SearchResult> HandleExactNumberSearch(ExactNumberSearchQueryDto query)
    {
        var results = _intervalTree.Search(query.Value, query.Value, (x, y) => x.Value.CompareTo(y));

        results.Sort((a, b) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new SearchMetadata(query.Boost));
        }
    }

    private IEnumerable<SearchResult> HandleGreaterNumberSearch(GreaterNumberSearchQueryDto query)
    {
        var results = _intervalTree.Search(query.Value, double.MaxValue, (x, y) => x.Value.CompareTo(y));

        results.Sort((a, b) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new SearchMetadata(query.Boost));
        }
    }

    private IEnumerable<SearchResult> HandleLessNumberSearch(LessNumberSearchQueryDto query)
    {
        var results = _intervalTree.Search(double.MinValue, query.Value, (x, y) => x.Value.CompareTo(y));

        results.Sort((a, b) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new SearchMetadata(query.Boost));
        }
    }

    private IEnumerable<SearchResult> HandleRangeNumberSearch(RangeNumberSearchQueryDto query)
    {
        var results = _intervalTree.Search(query.MinValue, query.MaxValue, (x, y) => x.Value.CompareTo(y));

        results.Sort((a, b) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new SearchMetadata(query.Boost));
        }
    }
}
