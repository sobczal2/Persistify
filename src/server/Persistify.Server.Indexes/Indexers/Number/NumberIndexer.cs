using System;
using System.Collections.Generic;
using Persistify.Domain.Documents;
using Persistify.Domain.Search.Queries;
using Persistify.Domain.Search.Queries.Number;
using Persistify.Server.ErrorHandling;
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
        var numberFieldValue = document.NumberFieldValuesByFieldName[FieldName];
        _intervalTree.Insert(new NumberIndexerIntervalTreeRecord
        {
            DocumentId = document.Id, Value = numberFieldValue.Value
        });
    }

    public IEnumerable<ISearchResult> SearchAsync(SearchQuery query)
    {
        if (query is not NumberSearchQuery numberSearchQuery || numberSearchQuery.GetFieldName() != FieldName)
        {
            throw new Exception("Invalid search query");
        }

        return query switch
        {
            ExactNumberSearchQuery exactNumberSearchQuery => HandleExactNumberSearch(exactNumberSearchQuery),
            GreaterNumberSearchQuery greaterNumberSearchQuery => HandleGreaterNumberSearch(greaterNumberSearchQuery),
            LessNumberSearchQuery lessNumberSearchQuery => HandleLessNumberSearch(lessNumberSearchQuery),
            RangeNumberSearchQuery rangeNumberSearchQuery => HandleRangeNumberSearch(rangeNumberSearchQuery),
            _ => throw new InternalPersistifyException(message: "Invalid search query")
        };
    }

    public void DeleteAsync(Document document)
    {
        _intervalTree.Remove(x => x.DocumentId == document.Id);
    }

    private IEnumerable<ISearchResult> HandleExactNumberSearch(ExactNumberSearchQuery query)
    {
        var results = _intervalTree.Search(query.Value, query.Value, (x, y) => x.Value.CompareTo(y));

        results.Sort((a, b) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new Metadata(query.Boost));
        }
    }

    private IEnumerable<ISearchResult> HandleGreaterNumberSearch(GreaterNumberSearchQuery query)
    {
        var results = _intervalTree.Search(query.Value, double.MaxValue, (x, y) => x.Value.CompareTo(y));

        results.Sort((a, b) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new Metadata(query.Boost));
        }
    }

    private IEnumerable<ISearchResult> HandleLessNumberSearch(LessNumberSearchQuery query)
    {
        var results = _intervalTree.Search(double.MinValue, query.Value, (x, y) => x.Value.CompareTo(y));

        results.Sort((a, b) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new Metadata(query.Boost));
        }
    }

    private IEnumerable<ISearchResult> HandleRangeNumberSearch(RangeNumberSearchQuery query)
    {
        var results = _intervalTree.Search(query.MinValue, query.MaxValue, (x, y) => x.Value.CompareTo(y));

        results.Sort((a, b) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new Metadata(query.Boost));
        }
    }
}
