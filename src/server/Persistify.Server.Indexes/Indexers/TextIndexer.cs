using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Search.Queries;
using Persistify.Domain.Search.Queries.Text;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers;

public class TextIndexer : IIndexer
{
    public SortedList<int, string> _documentValues;

    public TextIndexer(string fieldName)
    {
        FieldName = fieldName;
        _documentValues = new SortedList<int, string>();
    }

    public string FieldName { get; }

    public ValueTask IndexAsync(Document document)
    {
        var textFieldValue = document.TextFieldValuesByFieldName[FieldName];
        _documentValues.Add(document.Id, textFieldValue.Value);

        return ValueTask.CompletedTask;
    }

    public ValueTask<List<ISearchResult>> SearchAsync(SearchQuery query)
    {
        if (query is not TextSearchQuery textSearchQuery || textSearchQuery.GetFieldName() != FieldName)
        {
            throw new Exception("Invalid search query");
        }

        return textSearchQuery switch
        {
            ExactTextSearchQuery exactTextSearchQuery => HandleExactTextSearch(exactTextSearchQuery),
            _ => throw new Exception("Invalid search query")
        };
    }

    public ValueTask DeleteAsync(Document document)
    {
        _documentValues.Remove(document.Id);

        return ValueTask.CompletedTask;
    }

    private ValueTask<List<ISearchResult>> HandleExactTextSearch(ExactTextSearchQuery query)
    {
        return ValueTask.FromResult(
            _documentValues
                .Where(x => x.Value == query.Value)
                .Select(x => new SearchResult(x.Key, query.Boost) as ISearchResult)
                .ToList()
        );
    }
}
