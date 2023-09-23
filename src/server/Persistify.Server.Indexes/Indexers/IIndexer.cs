using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers;

public interface IIndexer
{
    IndexerKey Key { get; }
    ValueTask IndexAsync(Document document);
    ValueTask<IEnumerable<ISearchResult>> SearchAsync(ISearchQuery query);
    ValueTask DeleteAsync(Document document);
}
