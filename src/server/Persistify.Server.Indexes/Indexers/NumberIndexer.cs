using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Search.Queries;
using Persistify.Domain.Search.Queries.Number;
using Persistify.Server.ErrorHandling;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers;

public class NumberIndexer : IIndexer
{
    public SortedList<int, double> _documentValues;

    public NumberIndexer(string fieldName)
    {
        FieldName = fieldName;
        _documentValues = new SortedList<int, double>();
    }

    public string FieldName { get; }

    public ValueTask IndexAsync(Document document)
    {
        var numberFieldValue = document.NumberFieldValuesByFieldName[FieldName];
        _documentValues.Add(document.Id, numberFieldValue.Value);

        return ValueTask.CompletedTask;
    }

    public ValueTask<List<ISearchResult>> SearchAsync(SearchQuery query)
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
            _ => throw new PersistifyInternalException()
        };
    }

    public ValueTask DeleteAsync(Document document)
    {
        _documentValues.Remove(document.Id);

        return ValueTask.CompletedTask;
    }

    private ValueTask<List<ISearchResult>> HandleExactNumberSearch(ExactNumberSearchQuery query)
    {
        return ValueTask.FromResult(
            _documentValues
                .Where(x => x.Value == query.Value)
                .Select(x => new SearchResult(x.Key, query.Boost) as ISearchResult)
                .ToList()
        );
    }

    private ValueTask<List<ISearchResult>> HandleGreaterNumberSearch(GreaterNumberSearchQuery query)
    {
        return ValueTask.FromResult(
            _documentValues
                .Where(x => x.Value > query.Value)
                .Select(x => new SearchResult(x.Key, query.Boost) as ISearchResult)
                .ToList()
        );
    }

    private ValueTask<List<ISearchResult>> HandleLessNumberSearch(LessNumberSearchQuery query)
    {
        return ValueTask.FromResult(
            _documentValues
                .Where(x => x.Value < query.Value)
                .Select(x => new SearchResult(x.Key, query.Boost) as ISearchResult)
                .ToList()
        );
    }

    private ValueTask<List<ISearchResult>> HandleRangeNumberSearch(RangeNumberSearchQuery query)
    {
        return ValueTask.FromResult(
            _documentValues
                .Where(x => x.Value > query.MinValue &&
                            x.Value < query.MaxValue)
                .Select(x => new SearchResult(x.Key, query.Boost) as ISearchResult)
                .ToList()
        );
    }
}
