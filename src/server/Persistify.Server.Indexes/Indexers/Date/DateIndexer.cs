using System;
using System.Collections.Generic;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Dtos.Documents.Search.Queries.Date;
using Persistify.Dtos.Documents.Search.Queries.Number;
using Persistify.Server.Domain.Documents;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Indexes.DataStructures.Trees;
using Persistify.Server.Indexes.Indexers.Common;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers.Date;

public class DateIndexer : IIndexer
{
    private readonly IntervalTree<DateIndexerIntervalTreeRecord> _intervalTree;

    public DateIndexer(string fieldName)
    {
        FieldName = fieldName;
        _intervalTree = new IntervalTree<DateIndexerIntervalTreeRecord>();
    }

    public string FieldName { get; }

    public void Delete(Document document)
    {
        if (_intervalTree.Remove(x => x.DocumentId == document.Id) == 0)
        {
            throw new InternalPersistifyException();
        }
    }

    public void Index(Document document)
    {
        var dateFieldValue = document.GetDateFieldValueByName(FieldName);
        if (dateFieldValue is null)
        {
            throw new InternalPersistifyException();
        }

        _intervalTree.Insert(
            new DateIndexerIntervalTreeRecord
            {
                DocumentId = document.Id,
                Value = dateFieldValue.Value
            }
        );
    }

    public IEnumerable<SearchResult> Search(SearchQueryDto queryDto)
    {
        if (
            queryDto is not DateSearchQueryDto dateSearchQueryDto
            || dateSearchQueryDto.GetFieldName() != FieldName
        )
        {
            throw new InternalPersistifyException();
        }

        return dateSearchQueryDto switch
        {
            ExactDateSearchQueryDto exactDateSearchQueryDto
                => HandleExactDateSearch(exactDateSearchQueryDto),
            GreaterDateSearchQueryDto greaterDateSearchQueryDto
                => HandleGreaterDateSearch(greaterDateSearchQueryDto),
            LessDateSearchQueryDto lessDateSearchQueryDto
                => HandleLessDateSearch(lessDateSearchQueryDto),
            RangeDateSearchQueryDto rangeDateSearchQueryDto
                => HandleRangeDateSearch(rangeDateSearchQueryDto),
            _ => throw new InternalPersistifyException()
        };
    }

    private IEnumerable<SearchResult> HandleExactDateSearch(ExactDateSearchQueryDto queryDto)
    {
        var results = _intervalTree.Search(
            queryDto.Value,
            queryDto.Value,
            (x, y) => x.Value.CompareTo(y)
        );

        results.Sort((a, b) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new SearchMetadata(queryDto.Boost));
        }
    }

    private IEnumerable<SearchResult> HandleGreaterDateSearch(GreaterDateSearchQueryDto queryDto)
    {
        var results = _intervalTree.Search(
            queryDto.Value,
            queryDto.Value,
            (x, y) => x.Value.CompareTo(y)
        );

        results.Sort((a, b) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new SearchMetadata(queryDto.Boost));
        }
    }

    private IEnumerable<SearchResult> HandleLessDateSearch(LessDateSearchQueryDto queryDto)
    {
        var results = _intervalTree.Search(
            queryDto.Value,
            queryDto.Value,
            (x, y) => x.Value.CompareTo(y)
        );

        results.Sort((a, b) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new SearchMetadata(queryDto.Boost));
        }
    }

    private IEnumerable<SearchResult> HandleRangeDateSearch(
        RangeDateSearchQueryDto rangeDateSearchQueryDto
    )
    {
        var results = _intervalTree.Search(
            rangeDateSearchQueryDto.MinValue,
            rangeDateSearchQueryDto.MaxValue,
            (x, y) => x.Value.CompareTo(y)
        );

        results.Sort((a, b) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(
                result.DocumentId,
                new SearchMetadata(rangeDateSearchQueryDto.Boost)
            );
        }
    }
}
