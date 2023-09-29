using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Server.Indexes.Searches;
using Persistify.Server.Indexes.Searches.Queries.Text;

namespace Persistify.Server.Indexes.Indexers;

public class TextIndexer : IIndexer
{
    public string FieldName { get; }
    public SortedList<int, string> _documentValues;

    public TextIndexer(string fieldName)
    {
        FieldName = fieldName;
        _documentValues = new SortedList<int, string>();
    }

    public ValueTask IndexAsync(Document document)
    {
        var textFieldValue = document.TextFieldValuesByFieldName[FieldName];
        _documentValues.Add(document.Id, textFieldValue.Value);

        return ValueTask.CompletedTask;
    }

    public ValueTask<List<ISearchResult>> SearchAsync(ISearchQuery query)
    {
        if (query is not TextSearchQuery textSearchQuery || textSearchQuery.FieldName != FieldName)
        {
            throw new System.Exception("Invalid search query");
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
