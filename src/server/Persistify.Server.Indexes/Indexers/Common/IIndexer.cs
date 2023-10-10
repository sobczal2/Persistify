using System.Collections.Generic;
using Persistify.Domain.Documents;
using Persistify.Domain.Search.Queries;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers.Common;

public interface IIndexer
{
    string FieldName { get; }
    void Initialize(IEnumerable<Document> documents);
    void IndexAsync(Document document);
    IEnumerable<ISearchResult> SearchAsync(SearchQuery query);
    void DeleteAsync(Document document);
}
