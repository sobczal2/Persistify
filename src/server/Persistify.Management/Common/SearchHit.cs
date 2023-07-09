namespace Persistify.Management.Common;

public class SearchHit
{
    public SearchHit(long documentId, float boost)
    {
        DocumentId = documentId;
        Boost = boost;
    }

    public long DocumentId { get; set; }
    public float Boost { get; set; }
}
