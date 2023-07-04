using Persistify.Management.Common;

namespace Persistify.Management.Number.Search;

public class NumberSearchHit : SearchHit
{
    public NumberSearchHit(ulong id, float boost) : base(id, boost)
    {
    }
}
