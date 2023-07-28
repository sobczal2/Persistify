using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Types.Number.Queries;

public class EqualNumberManagerQuery : NumberManagerQuery
{
    public double Value { get; }

    public EqualNumberManagerQuery(
        TemplateFieldIdentifier templateFieldIdentifier,
        double value
        ) : base(templateFieldIdentifier)
    {
        Value = value;
    }
}
