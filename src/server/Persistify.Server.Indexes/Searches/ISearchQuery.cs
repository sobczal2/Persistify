using Persistify.Server.Indexes.Indexers;

namespace Persistify.Server.Indexes.Searches;

public interface ISearchQuery
{
    IndexerKey IndexerKey { get; }
    float Boost { get; }
}
