using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Types.Number.Queries;

public class RangeNumberManagerQuery : NumberManagerQuery
{
    public double MinValue { get; }
    public double MaxValue { get; }

    public RangeNumberManagerQuery(TemplateFieldIdentifier templateFieldIdentifier, double minValue, double maxValue) :
        base(templateFieldIdentifier)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }
}
