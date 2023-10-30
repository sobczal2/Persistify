using System;
using System.Collections;
using System.Collections.Generic;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Dtos.Documents.Search.Queries.Bool;
using Persistify.Helpers.Collections;
using Persistify.Server.Domain.Documents;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Indexes.Indexers.Common;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers.Bool;

public class BoolIndexer : IIndexer
{
    private readonly BitArray _trueDocuments;
    private readonly BitArray _falseDocuments;

    public BoolIndexer(string fieldName)
    {
        FieldName = fieldName;
        _trueDocuments = new BitArray(0);
        _falseDocuments = new BitArray(0);
    }

    public string FieldName { get; }

    public void Index(Document document)
    {
        var boolFieldValue = document.GetBoolFieldValueByName(FieldName);
        if (boolFieldValue == null)
        {
            throw new InternalPersistifyException();
        }

        if (boolFieldValue.Value)
        {
            _trueDocuments.SetEnsureCapacity(document.Id, true);
        }
        else
        {
            _falseDocuments.SetEnsureCapacity(document.Id, true);
        }
    }

    public IEnumerable<SearchResult> Search(SearchQueryDto queryDto)
    {
        if (
            queryDto is not BoolSearchQueryDto boolSearchQueryDto
            || boolSearchQueryDto.GetFieldName() != FieldName
        )
        {
            throw new Exception("Invalid search query");
        }

        return boolSearchQueryDto switch
        {
            ExactBoolSearchQueryDto exactBoolSearchQueryDto
                => HandleExactBoolSearch(exactBoolSearchQueryDto),
            _ => throw new InternalPersistifyException(message: "Invalid search query")
        };
    }

    public void Delete(Document document)
    {
        _trueDocuments.SetEnsureCapacity(document.Id, false);
        _falseDocuments.SetEnsureCapacity(document.Id, false);
    }

    private IEnumerable<SearchResult> HandleExactBoolSearch(ExactBoolSearchQueryDto queryDto)
    {
        if (queryDto.Value)
        {
            for (var i = 0; i < _trueDocuments.Length; i++)
            {
                if (_trueDocuments[i])
                {
                    yield return new SearchResult(i, new SearchMetadata(queryDto.Boost));
                }
            }
        }
        else
        {
            for (var i = 0; i < _falseDocuments.Length; i++)
            {
                if (_falseDocuments[i])
                {
                    yield return new SearchResult(i, new SearchMetadata(queryDto.Boost));
                }
            }
        }
    }
}
