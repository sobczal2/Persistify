using Microsoft.Extensions.Options;
using Persistify.Cache;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Management.Document.Cache;

public class DocumentCache : LruCache<(string templateName, long id), Protos.Documents.Shared.Document>, IDocumentCache
{
    public DocumentCache(IOptions<CacheSettings> options) : base(options.Value.DocumentCacheCapacity)
    {
    }
}
