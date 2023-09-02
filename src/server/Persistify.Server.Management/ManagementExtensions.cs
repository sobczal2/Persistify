using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Management.Files;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Managers.Templates;

namespace Persistify.Server.Management;

public static class ManagementExtensions
{
    public static IServiceCollection AddManagement(this IServiceCollection services)
    {
        services.AddSingleton<ITemplateManager, TemplateManager>();
        services.AddSingleton<IDocumentManager, DocumentManager>();
        services.AddSingleton<IFileStreamFactory, IdleTimeoutFileStreamFactory>();
        services.AddSingleton<IFileManager, FileManager>();
        services.AddSingleton<IFileProvider, LocalFileProvider>();
        services.AddSingleton<IRequiredFileGroup, TemplateManagerRequiredFileGroup>();
        services.AddSingleton<IDocumentManagerStore, DocumentManagerStore>();

        return services;
    }
}
