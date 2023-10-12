using System;
using System.Collections.Generic;
using Persistify.Domain.Documents;
using Persistify.Domain.Search.Queries;
using Persistify.Domain.Search.Queries.Bool;
using Persistify.Server.ErrorHandling;
using Persistify.Server.Indexes.Indexers.Common;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers.Bool;

public class BoolIndexer : IIndexer
{
    private readonly SortedSet<int> _falseDocuments;

    private readonly SortedSet<int> _trueDocuments;

    public BoolIndexer(string fieldName)
    {
        FieldName = fieldName;
        _trueDocuments = new SortedSet<int>();
        _falseDocuments = new SortedSet<int>();
    }

    public string FieldName { get; }

    public void Initialize(IEnumerable<Document> documents)
    {
        foreach (var document in documents)
        {
            var boolFieldValue = document.BoolFieldValuesByFieldName[FieldName];
            if (boolFieldValue.Value)
            {
                _trueDocuments.Add(document.Id);
            }
            else
            {
                _falseDocuments.Add(document.Id);
            }
        }
    }

    public void IndexAsync(Document document)
    {
        var boolFieldValue = document.BoolFieldValuesByFieldName[FieldName];
        if (boolFieldValue.Value)
        {
            _trueDocuments.Add(document.Id);
        }
        else
        {
            _falseDocuments.Add(document.Id);
        }
    }

    public IEnumerable<ISearchResult> SearchAsync(SearchQuery query)
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

    public void DeleteAsync(Document document)
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
    }

    private IEnumerable<ISearchResult> HandleExactBoolSearch(ExactBoolSearchQuery query)
    {
        if (query.Value)
        {
            foreach (var documentId in _trueDocuments)
            {
                yield return new SearchResult(documentId, new Metadata(query.Boost));
            }
        }
        else
        {
            foreach (var documentId in _falseDocuments)
            {
                yield return new SearchResult(documentId, new Metadata(query.Boost));
            }
        }
    }
}
