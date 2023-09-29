using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Search.Queries;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers;

public interface IIndexer
{
    string FieldName { get; }
    ValueTask IndexAsync(Document document);
    ValueTask<List<ISearchResult>> SearchAsync(SearchQuery query);
    ValueTask DeleteAsync(Document document);
}
