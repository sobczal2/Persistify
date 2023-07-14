using Persistify.Cache;

namespace Persistify.Management.Template.Cache;

public interface ITemplateCache : ICache<string, Protos.Templates.Shared.Template>
{
}
