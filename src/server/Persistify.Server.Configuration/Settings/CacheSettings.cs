namespace Persistify.Server.Configuration.Settings;

public class CacheSettings
{
    public const string SectionName = "Cache";

    public int TemplateCacheCapacity { get; set; }
    public int DocumentCacheCapacity { get; set; }
}
