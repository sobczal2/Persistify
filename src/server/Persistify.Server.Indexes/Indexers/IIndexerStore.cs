using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers;

public interface IIndexerStore
{
    ValueTask InitializeAsync();
    ValueTask<IEnumerable<ISearchResult>> Search(ISearchQuery query);
    void Add(IndexerKey key);
    void Remove(IndexerKey key);
}
