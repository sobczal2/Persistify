using Microsoft.Extensions.DependencyInjection;
using Persistify.Persistance.Document;
using Persistify.Persistance.Template;

namespace Persistify.Persistance;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddSingleton<ITemplateStorage, FileSystemTemplateStorage>();
        services.AddSingleton<IDocumentStorage, FileSystemDocumentStorage>();

        return services;
    }
}
