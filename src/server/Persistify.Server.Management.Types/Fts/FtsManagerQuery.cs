using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Types.Fts;

public abstract class FtsManagerQuery : ITypeManagerQuery
{
    protected FtsManagerQuery(TemplateFieldIdentifier templateFieldIdentifier, string value)
    {
        TemplateFieldIdentifier = templateFieldIdentifier;
        Value = value;
    }

    public string Value { get; }
    public TemplateFieldIdentifier TemplateFieldIdentifier { get; }
}
