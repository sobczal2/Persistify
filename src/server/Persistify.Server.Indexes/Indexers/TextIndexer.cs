using System;
using System.Collections.Generic;
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
        if (query is not TextSearchQuery textSearchQuery || textSearchQuery.FieldName != FieldName)
        {
            throw new Exception("Invalid search query");
        }

        var results = new List<ISearchResult>();

        return ValueTask.FromResult(results);
    }

    public ValueTask DeleteAsync(Document document)
    {
        var textFieldValue = document.TextFieldValuesByFieldName[FieldName];
        _documentValues.Remove(document.Id);

        return ValueTask.CompletedTask;
    }
}
