using Persistify.Server.Management.Types.Abstractions;

namespace Persistify.Server.Management.Types.Text;

public class TextManagerQuery : ITypeManagerQuery
{
    public TextManagerQuery(TemplateFieldIdentifier templateFieldIdentifier, string value)
    {
        TemplateFieldIdentifier = templateFieldIdentifier;
        Value = value;
    }

    public string Value { get; }
    public TemplateFieldIdentifier TemplateFieldIdentifier { get; }
}
