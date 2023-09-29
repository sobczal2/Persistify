using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Server.Indexes.Searches;
using Persistify.Server.Indexes.Searches.Queries.Bool;

namespace Persistify.Server.Indexes.Indexers;

public class BoolIndexer : IIndexer
{
    public string FieldName { get; }
    private readonly SortedList<int, int> _trueDocuments;
    private readonly SortedList<int, int> _falseDocuments;

    public BoolIndexer(string fieldName)
    {
        FieldName = fieldName;
        _trueDocuments = new SortedList<int, int>();
        _falseDocuments = new SortedList<int, int>();
    }

    public ValueTask IndexAsync(Document document)
    {
        var boolFieldValue = document.BoolFieldValuesByFieldName[FieldName];
        if (boolFieldValue.Value)
        {
            _trueDocuments.Add(document.Id, document.Id);
        }
        else
        {
            _falseDocuments.Add(document.Id, document.Id);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask<List<ISearchResult>> SearchAsync(ISearchQuery query)
    {
        if (query is not BoolSearchQuery boolSearchQuery || boolSearchQuery.FieldName != FieldName)
        {
            throw new System.Exception("Invalid search query");
        }

        if (boolSearchQuery.Value)
        {
            return ValueTask.FromResult(_trueDocuments.Select(x => new SearchResult(x.Key, query.Boost)).Cast<ISearchResult>().ToList());
        }
        else
        {
            return ValueTask.FromResult(_falseDocuments.Select(x => new SearchResult(x.Key, query.Boost)).Cast<ISearchResult>().ToList());
        }
    }

    public ValueTask DeleteAsync(Document document)
    {
        var boolFieldValue = document.BoolFieldValuesByFieldName[FieldName];
        if (boolFieldValue.Value)
        {
            _trueDocuments.Remove(document.Id);
        }
        else
        {
            _falseDocuments.Remove(document.Id);
        }

        return ValueTask.CompletedTask;
    }
}
