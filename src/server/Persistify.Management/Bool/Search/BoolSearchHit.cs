using Persistify.Management.Common;

namespace Persistify.Management.Bool.Search;

public class BoolSearchHit : SearchHit
{
    public BoolSearchHit(ulong documentId, float boost) : base(documentId, boost)
    {
    }
}
