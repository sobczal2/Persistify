using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Types.Number.Queries;

public class GreaterThanOrEqualNumberManagerQuery : NumberManagerQuery
{
    public double Value { get; }

    public GreaterThanOrEqualNumberManagerQuery(
        TemplateFieldIdentifier templateFieldIdentifier,
        double value
        ) : base(
        templateFieldIdentifier
        )
    {
        Value = value;
    }
}
