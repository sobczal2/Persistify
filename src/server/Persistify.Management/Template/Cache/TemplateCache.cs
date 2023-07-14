using Microsoft.Extensions.Options;
using Persistify.Cache;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Management.Template.Cache;

public class TemplateCache : LruCache<string, Protos.Templates.Shared.Template>, ITemplateCache
{
    public TemplateCache(IOptions<CacheSettings> cacheSettings) : base(cacheSettings.Value.TemplateCacheCapacity)
    {
    }
}
