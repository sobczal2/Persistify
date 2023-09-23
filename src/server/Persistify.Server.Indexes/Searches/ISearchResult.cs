using System.Collections.Generic;

namespace Persistify.Server.Indexes.Searches;

public interface ISearchResult
{
    int DocumentId { get; }
    float Score { get; }
    List<IMetadata> Metadata { get; }
}
