using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Types.Number.Queries;

public class LessThanOrEqualNumberManagerQuery : NumberManagerQuery
{
    public double Value { get; }

    public LessThanOrEqualNumberManagerQuery(
        TemplateFieldIdentifier templateFieldIdentifier,
        double value
        ) : base(
        templateFieldIdentifier)
    {
        Value = value;
    }
}
