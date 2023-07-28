using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Types.Number.Queries;

public class GreaterThanNumberManagerQuery : NumberManagerQuery
{
    public double Value { get; }

    public GreaterThanNumberManagerQuery(
        TemplateFieldIdentifier templateFieldIdentifier,
        double value
        ) : base(templateFieldIdentifier)
    {
        Value = value;
    }
}
