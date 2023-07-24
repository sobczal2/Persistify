using Persistify.Server.Management.Types.Abstractions;
using Persistify.Server.Management.Types.Shared;

namespace Persistify.Server.Management.Types.Bool;

public class BoolManagerQuery : ITypeManagerQuery
{
    public BoolManagerQuery(TemplateFieldIdentifier templateFieldIdentifier, bool value)
    {
        TemplateFieldIdentifier = templateFieldIdentifier;
        Value = value;
    }

    public bool Value { get; }
    public TemplateFieldIdentifier TemplateFieldIdentifier { get; }
}
