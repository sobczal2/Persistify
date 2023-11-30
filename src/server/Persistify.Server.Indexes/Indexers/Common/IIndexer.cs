using System.Collections.Generic;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Server.Domain.Documents;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers.Common;

public interface IIndexer
{
    string FieldName { get; }

    void Index(
        Document document
    );

    IEnumerable<SearchResult> Search(
        SearchQueryDto queryDto
    );

    void Delete(
        Document document
    );
}
