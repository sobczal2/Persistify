using Persistify.Management.Types.Abstractions;
using Persistify.Management.Types.Shared;

namespace Persistify.Management.Types.Bool;

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
