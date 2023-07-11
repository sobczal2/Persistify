using Persistify.Management.Common;

namespace Persistify.Management.Fts.Search;

public class FtsSearchHit : SearchHit
{
    public FtsSearchHit(long id, float boost) : base(id, boost)
    {
    }
}
