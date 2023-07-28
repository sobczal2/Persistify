using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Types.Number.Queries;

public abstract class NumberManagerQuery : ITypeManagerQuery
{
    protected NumberManagerQuery(TemplateFieldIdentifier templateFieldIdentifier)
    {
        TemplateFieldIdentifier = templateFieldIdentifier;
    }
    public TemplateFieldIdentifier TemplateFieldIdentifier { get; }
}
