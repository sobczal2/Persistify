using Persistify.Server.Management.Types.Abstractions;
using Persistify.Server.Management.Types.Shared;

namespace Persistify.Server.Management.Types.Number;

public class NumberManagerQuery : ITypeManagerQuery
{
    public NumberManagerQuery(TemplateFieldIdentifier templateFieldIdentifier, double minValue, double maxValue)
    {
        TemplateFieldIdentifier = templateFieldIdentifier;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public double MinValue { get; }
    public double MaxValue { get; }
    public TemplateFieldIdentifier TemplateFieldIdentifier { get; }
}
