using Persistify.Management.Types.Abstractions;
using Persistify.Management.Types.Shared;

namespace Persistify.Management.Types.Text;

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
