using System;
using System.Collections.Generic;
using Persistify.Dtos.Documents.Search.Queries;
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
        _intervalTree.Remove(x => x.DocumentId == document.Id);
    }

    public void Index(Document document)
    {
        var dateFieldValue = document.GetDateFieldValueByName(FieldName);
        if (dateFieldValue == null)
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
        throw new NotImplementedException();
    }
}
