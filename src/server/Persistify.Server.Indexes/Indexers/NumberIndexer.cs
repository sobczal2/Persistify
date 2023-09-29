using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Search.Queries;
using Persistify.Domain.Search.Queries.Number;
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
        if (query is not NumberSearchQuery numberSearchQuery || numberSearchQuery.FieldName != FieldName)
        {
            throw new Exception("Invalid search query");
        }

        switch (query)
        {
            case EqualNumberSearchQuery equalNumberSearchQuery:
                return ValueTask.FromResult(
                    _documentValues
                        .Where(x => x.Value == equalNumberSearchQuery.Value)
                        .Select(x => new SearchResult(x.Key, equalNumberSearchQuery.Boost) as ISearchResult)
                        .ToList()
                );
            case GreaterNumberSearchQuery greaterNumberSearchQuery:
                return ValueTask.FromResult(
                    _documentValues
                        .Where(x => x.Value > greaterNumberSearchQuery.Value)
                        .Select(x => new SearchResult(x.Key, greaterNumberSearchQuery.Boost) as ISearchResult)
                        .ToList()
                );
            case LessNumberSearchQuery lessNumberSearchQuery:
                return ValueTask.FromResult(
                    _documentValues
                        .Where(x => x.Value < lessNumberSearchQuery.Value)
                        .Select(x => new SearchResult(x.Key, lessNumberSearchQuery.Boost) as ISearchResult)
                        .ToList()
                );
            case RangeNumberSearchQuery rangeNumberSearchQuery:
                return ValueTask.FromResult(
                    _documentValues
                        .Where(x => x.Value < rangeNumberSearchQuery.MinValue &&
                                    x.Value > rangeNumberSearchQuery.MaxValue)
                        .Select(x => new SearchResult(x.Key, rangeNumberSearchQuery.Boost) as ISearchResult)
                        .ToList()
                );
            default:
                throw new Exception("Invalid search query");
        }
    }

    public ValueTask DeleteAsync(Document document)
    {
        throw new NotImplementedException();
    }
}
