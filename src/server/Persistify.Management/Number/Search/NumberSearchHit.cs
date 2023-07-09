using Persistify.Management.Common;

namespace Persistify.Management.Number.Search;

public class NumberSearchHit : SearchHit
{
    public NumberSearchHit(long id, float boost) : base(id, boost)
    {
    }
}
