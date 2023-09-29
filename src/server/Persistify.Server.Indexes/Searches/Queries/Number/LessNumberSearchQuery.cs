namespace Persistify.Server.Indexes.Searches.Queries.Number;

public class LessNumberSearchQuery : NumberSearchQuery
{
    public double Value { get; }

    public LessNumberSearchQuery(
        string fieldName,
        float boost,
        double value
    ) : base(
        fieldName,
        boost
    )
    {
        Value = value;
    }
}
