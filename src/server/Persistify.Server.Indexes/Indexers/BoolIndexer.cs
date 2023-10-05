using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Search.Queries;
using Persistify.Domain.Search.Queries.Bool;
using Persistify.Server.ErrorHandling;
using Persistify.Server.Indexes.Searches;
using Persistify.Server.Persistence.Primitives;

namespace Persistify.Server.Indexes.Indexers;

public class BoolIndexer : IIndexer
{
    private readonly BoolStreamRepository _trueDocuments;
    private readonly BoolStreamRepository _falseDocuments;

    public BoolIndexer(string fieldName, Stream trueStream, Stream falseStream)
    {
        FieldName = fieldName;
        _trueDocuments = new BoolStreamRepository(trueStream);
        _falseDocuments = new BoolStreamRepository(falseStream);
    }

    public string FieldName { get; }

    public async ValueTask IndexAsync(Document document)
    {
        var boolFieldValue = document.BoolFieldValuesByFieldName[FieldName];
        if (boolFieldValue.Value)
        {
            await _trueDocuments.WriteAsync(document.Id, true, true);
        }
        else
        {
            await _falseDocuments.WriteAsync(document.Id, true, true);
        }
    }

    public ValueTask<List<ISearchResult>> SearchAsync(SearchQuery query)
    {
        if (query is not BoolSearchQuery boolSearchQuery || boolSearchQuery.GetFieldName() != FieldName)
        {
            throw new Exception("Invalid search query");
        }

        return boolSearchQuery switch
        {
            ExactBoolSearchQuery exactBoolSearchQuery => HandleExactBoolSearch(exactBoolSearchQuery),
            _ => throw new PersistifyInternalException()
        };
    }

    public async ValueTask DeleteAsync(Document document)
    {
        var boolFieldValue = document.BoolFieldValuesByFieldName[FieldName];
        if (boolFieldValue.Value)
        {
            await _trueDocuments.DeleteAsync(document.Id, true);
        }
        else
        {
            await _falseDocuments.DeleteAsync(document.Id, true);
        }
    }

    private async ValueTask<List<ISearchResult>> HandleExactBoolSearch(ExactBoolSearchQuery query)
    {
        if (query.Value)
        {
            var trueValues = await _trueDocuments.ReadRangeAsync(int.MaxValue, 0, true);
            var result = new List<ISearchResult>(trueValues.Count);
            foreach (var (key, _) in trueValues)
            {
                result.Add(new SearchResult(key, query.Boost));
            }

            return result;
        }

        var falseValues = await _falseDocuments.ReadRangeAsync(int.MaxValue, 0, true);
        var falseResult = new List<ISearchResult>(falseValues.Count);
        foreach (var (key, _) in falseValues)
        {
            falseResult.Add(new SearchResult(key, query.Boost));
        }

        return falseResult;
    }
}
