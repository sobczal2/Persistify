using Microsoft.Extensions.DependencyInjection;
using Persistify.Cache;
using Persistify.Management.Document.Cache;
using Persistify.Management.Document.Manager;
using Persistify.Management.Template.Cache;
using Persistify.Management.Template.Manager;

namespace Persistify.Management;

public static class ManagementExtensions
{
    public static IServiceCollection AddManagement(this IServiceCollection services)
    {
        services.AddSingleton<ITemplateManager, DictionaryTemplateManager>();
        services.AddSingleton<IDocumentManager, DocumentManager>();

        services.AddCache<ITemplateCache, TemplateCache>();
        services.AddCache<IDocumentCache, DocumentCache>();

        return services;
    }
}
