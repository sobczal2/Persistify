namespace Persistify.Server.Indexes.Searches.Queries.Number;

public class RangeNumberSearchQuery : NumberSearchQuery
{
    public double MinValue { get; }
    public double MaxValue { get; }

    public RangeNumberSearchQuery(
        string fieldName,
        float boost,
        double minValue,
        double maxValue
    ) : base(
        fieldName,
        boost
    )
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }
}
