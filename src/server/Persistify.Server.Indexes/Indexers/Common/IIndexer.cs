using System.Collections.Generic;
using Persistify.Server.Domain.Documents;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers.Common;

public interface IIndexer
{
    string FieldName { get; }
    void IndexAsync(Document document);
    IEnumerable<SearchResult> SearchAsync(SearchQueryDto query);
    void DeleteAsync(Document document);
}
