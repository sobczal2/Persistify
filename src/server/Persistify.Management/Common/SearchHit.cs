namespace Persistify.Management.Common;

public class SearchHit
{
    public SearchHit(ulong documentId, float boost)
    {
        DocumentId = documentId;
        Boost = boost;
    }

    public ulong DocumentId { get; set; }
    public float Boost { get; set; }
}
