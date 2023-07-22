using Persistify.Management.Types.Abstractions;
using Persistify.Management.Types.Shared;

namespace Persistify.Management.Types.Bool;

public class BoolManagerQuery : ITypeManagerQuery
{
    public TemplateFieldIdentifier TemplateFieldIdentifier { get; }
    public bool Value { get; }

    public BoolManagerQuery(TemplateFieldIdentifier templateFieldIdentifier, bool value)
    {
        TemplateFieldIdentifier = templateFieldIdentifier;
        Value = value;
    }
}
