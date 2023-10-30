using System.Collections.Generic;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Server.Domain.Documents;
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
        throw new System.NotImplementedException();
    }

    public void Index(Document document)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<SearchResult> Search(SearchQueryDto queryDto)
    {
        throw new System.NotImplementedException();
    }
}
