using Persistify.Server.Management.Types.Abstractions;
using Persistify.Server.Management.Types.Shared;

namespace Persistify.Server.Management.Types.Fts;

public class FtsManagerQuery : ITypeManagerQuery
{
    public FtsManagerQuery(TemplateFieldIdentifier templateFieldIdentifier, string value, bool prefix, bool suffix)
    {
        TemplateFieldIdentifier = templateFieldIdentifier;
        Value = value;
        Prefix = prefix;
        Suffix = suffix;
    }

    public string Value { get; }
    public bool Prefix { get; }
    public bool Suffix { get; }
    public TemplateFieldIdentifier TemplateFieldIdentifier { get; }
}
