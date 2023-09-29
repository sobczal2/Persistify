namespace Persistify.Server.Indexes.Searches.Queries.Number;

public class EqualNumberSearchQuery : NumberSearchQuery
{
    public double Value { get; }
    public EqualNumberSearchQuery(
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
