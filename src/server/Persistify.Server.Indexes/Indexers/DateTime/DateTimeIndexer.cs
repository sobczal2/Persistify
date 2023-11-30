using System.Collections.Generic;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Dtos.Documents.Search.Queries.DateTime;
using Persistify.Server.Domain.Documents;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Indexes.DataStructures.Trees;
using Persistify.Server.Indexes.Indexers.Common;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers.DateTime;

public class DateTimeIndexer : IIndexer
{
    private readonly IntervalTree<DateTimeIndexerIntervalTreeRecord> _intervalTree;

    public DateTimeIndexer(
        string fieldName
    )
    {
        FieldName = fieldName;
        _intervalTree = new IntervalTree<DateTimeIndexerIntervalTreeRecord>();
    }

    public string FieldName { get; }

    public void Delete(
        Document document
    )
    {
        if (_intervalTree.Remove(x => x.DocumentId == document.Id) == 0)
        {
            throw new InternalPersistifyException();
        }
    }

    public void Index(
        Document document
    )
    {
        var dateFieldValue = document.GetDateTimeFieldValueByName(FieldName);
        if (dateFieldValue is null)
        {
            throw new InternalPersistifyException();
        }

        _intervalTree.Insert(
            new DateTimeIndexerIntervalTreeRecord { DocumentId = document.Id, Value = dateFieldValue.Value }
        );
    }

    public IEnumerable<SearchResult> Search(
        SearchQueryDto queryDto
    )
    {
        if (
            queryDto is not DateTimeSearchQueryDto dateSearchQueryDto
            || dateSearchQueryDto.GetFieldName() != FieldName
        )
        {
            throw new InternalPersistifyException();
        }

        return dateSearchQueryDto switch
        {
            ExactDateTimeSearchQueryDto exactDateSearchQueryDto
                => HandleExactDateTimeSearch(exactDateSearchQueryDto),
            GreaterDateTimeSearchQueryDto greaterDateSearchQueryDto
                => HandleGreaterDateTimeSearch(greaterDateSearchQueryDto),
            LessDateTimeSearchQueryDto lessDateSearchQueryDto
                => HandleLessDateTimeSearch(lessDateSearchQueryDto),
            RangeDateTimeSearchQueryDto rangeDateSearchQueryDto
                => HandleRangeDateTimeSearch(rangeDateSearchQueryDto),
            _ => throw new InternalPersistifyException()
        };
    }

    private IEnumerable<SearchResult> HandleExactDateTimeSearch(
        ExactDateTimeSearchQueryDto queryDto
    )
    {
        var results = _intervalTree.Search(
            queryDto.Value,
            queryDto.Value,
            (
                x,
                y
            ) => x.Value.CompareTo(y)
        );

        results.Sort((
            a,
            b
        ) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new SearchMetadata(queryDto.Boost));
        }
    }

    private IEnumerable<SearchResult> HandleGreaterDateTimeSearch(
        GreaterDateTimeSearchQueryDto queryDto
    )
    {
        var results = _intervalTree.Search(
            queryDto.Value,
            queryDto.Value,
            (
                x,
                y
            ) => x.Value.CompareTo(y)
        );

        results.Sort((
            a,
            b
        ) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new SearchMetadata(queryDto.Boost));
        }
    }

    private IEnumerable<SearchResult> HandleLessDateTimeSearch(
        LessDateTimeSearchQueryDto queryDto
    )
    {
        var results = _intervalTree.Search(
            queryDto.Value,
            queryDto.Value,
            (
                x,
                y
            ) => x.Value.CompareTo(y)
        );

        results.Sort((
            a,
            b
        ) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new SearchMetadata(queryDto.Boost));
        }
    }

    private IEnumerable<SearchResult> HandleRangeDateTimeSearch(
        RangeDateTimeSearchQueryDto rangeDateTimeSearchQueryDto
    )
    {
        var results = _intervalTree.Search(
            rangeDateTimeSearchQueryDto.MinValue,
            rangeDateTimeSearchQueryDto.MaxValue,
            (
                x,
                y
            ) => x.Value.CompareTo(y)
        );

        results.Sort((
            a,
            b
        ) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(
                result.DocumentId,
                new SearchMetadata(rangeDateTimeSearchQueryDto.Boost)
            );
        }
    }
}
