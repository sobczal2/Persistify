namespace Persistify.Server.Indexes.Searches.Queries.Text;

public class PrefixTextSearchQuery : TextSearchQuery
{
    public float Value { get; }

    public PrefixTextSearchQuery(
        string fieldName,
        float boost,
        float value
    ) : base(
        fieldName,
        boost
    )
    {
        Value = value;
    }
}
