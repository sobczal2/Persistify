using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Types.Number.Queries;

public class NotEqualNumberManagerQuery : NumberManagerQuery
{
    public double Value { get; }

    public NotEqualNumberManagerQuery(
        TemplateFieldIdentifier templateFieldIdentifier,
        double value
        ) : base(templateFieldIdentifier)
    {
        Value = value;
    }
}
