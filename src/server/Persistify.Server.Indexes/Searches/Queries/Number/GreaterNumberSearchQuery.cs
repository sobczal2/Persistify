namespace Persistify.Server.Indexes.Searches.Queries.Number;

public class GreaterNumberSearchQuery : NumberSearchQuery
{
    public double Value { get; }

    public GreaterNumberSearchQuery(
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
