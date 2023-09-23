using Persistify.Server.Indexes.Indexers;

namespace Persistify.Server.Indexes.Searches;

public interface ISearchQuery
{
    string FieldName { get; }
    float Boost { get; }
}
