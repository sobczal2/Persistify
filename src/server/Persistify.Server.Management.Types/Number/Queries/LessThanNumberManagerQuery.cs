using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Types.Number.Queries;

public class LessThanNumberManagerQuery : NumberManagerQuery
{
    public double Value { get; }

    public LessThanNumberManagerQuery(
        TemplateFieldIdentifier templateFieldIdentifier,
        double value
        ) : base(templateFieldIdentifier)
    {
        Value = value;
    }
}
