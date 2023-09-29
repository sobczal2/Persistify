namespace Persistify.Server.Indexes.Searches.Queries.Text;

public class FullTextSearchQuery : TextSearchQuery
{
    public string Value { get; }

    public FullTextSearchQuery(
        string fieldName,
        float boost,
        string value
    ) : base(
        fieldName,
        boost
    )
    {
        Value = value;
    }
}
