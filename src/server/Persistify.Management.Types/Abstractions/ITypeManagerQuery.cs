using Persistify.Management.Types.Shared;

namespace Persistify.Management.Types.Abstractions;

public interface ITypeManagerQuery
{
    TemplateFieldIdentifier TemplateFieldIdentifier { get; }
}
