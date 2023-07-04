using Persistify.Management.Common;

namespace Persistify.Management.Bool.Search;

public class BoolSearchHit : SearchHit
{
    public BoolSearchHit(ulong id, float boost) : base(id, boost)
    {
    }
}
