using System;
using System.Collections.Generic;
using Persistify.Server.Domain.Documents;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Dtos.Documents.Search.Queries.Bool;
using Persistify.Server.ErrorHandling.Exceptions;
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

    public void IndexAsync(Document document)
    {
        var boolFieldValue = document.GetBoolFieldValueByName(FieldName);
        if (boolFieldValue == null)
        {
            throw new InternalPersistifyException();
        }

        if (boolFieldValue.Value)
        {
            _trueDocuments.Add(document.Id);
        }
        else
        {
            _falseDocuments.Add(document.Id);
        }
    }

    public IEnumerable<SearchResult> SearchAsync(SearchQueryDto queryDto)
    {
        if (queryDto is not BoolSearchQueryDto boolSearchQueryDto || boolSearchQueryDto.GetFieldName() != FieldName)
        {
            throw new Exception("Invalid search query");
        }

        return boolSearchQueryDto switch
        {
            ExactBoolSearchQueryDto exactBoolSearchQueryDto => HandleExactBoolSearch(exactBoolSearchQueryDto),
            _ => throw new InternalPersistifyException(message: "Invalid search query")
        };
    }

    public void DeleteAsync(Document document)
    {
        _trueDocuments.Remove(document.Id);
        _falseDocuments.Remove(document.Id);
    }

    private IEnumerable<SearchResult> HandleExactBoolSearch(ExactBoolSearchQueryDto queryDto)
    {
        if (queryDto.Value)
        {
            foreach (var documentId in _trueDocuments)
            {
                yield return new SearchResult(documentId, new SearchMetadata(queryDto.Boost));
            }
        }
        else
        {
            foreach (var documentId in _falseDocuments)
            {
                yield return new SearchResult(documentId, new SearchMetadata(queryDto.Boost));
            }
        }
    }
}
