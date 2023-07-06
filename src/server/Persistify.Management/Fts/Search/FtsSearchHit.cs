using Persistify.Management.Common;

namespace Persistify.Management.Fts.Search;

public class FtsSearchHit : SearchHit
{
    public FtsSearchHit(ulong id, float boost) : base(id, boost)
    {
    }
}
