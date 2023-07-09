using Persistify.Management.Common;

namespace Persistify.Management.Bool.Search;

public class BoolSearchHit : SearchHit
{
    public BoolSearchHit(long documentId, float boost) : base(documentId, boost)
    {
    }
}
