namespace Persistify.Server.Indexes.Searches.Queries.Text;

public class ExactTextSearchQuery : TextSearchQuery
{
    public string Value { get; }

    public ExactTextSearchQuery(
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
