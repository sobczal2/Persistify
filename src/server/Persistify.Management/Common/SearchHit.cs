namespace Persistify.Management.Common;

public class SearchHit
{
    public ulong Id { get; set; }
    public float Boost { get; set; }

    public SearchHit(ulong id, float boost)
    {
        Id = id;
        Boost = boost;
    }
}
