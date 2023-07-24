using Persistify.Server.Management.Types.Shared;

namespace Persistify.Server.Management.Types.Abstractions;

public interface ITypeManagerQuery
{
    TemplateFieldIdentifier TemplateFieldIdentifier { get; }
}
