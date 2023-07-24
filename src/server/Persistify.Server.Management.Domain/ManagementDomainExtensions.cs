using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.HostedServices;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.Management.Domain.Abstractions;
using Persistify.Server.Management.Domain.Implementations;

namespace Persistify.Server.Management.Domain;

public static class ManagementDomainExtensions
{
    public static IServiceCollection AddManagementDomain(this IServiceCollection services)
    {
        services.AddSingleton<TemplateManager>();
        services.AddSingleton<ITemplateManager>(provider => provider.GetRequiredService<TemplateManager>());
        services.AddSingleton<IActOnStartup>(provider => provider.GetRequiredService<TemplateManager>());
        services.AddSingleton<DocumentIdManager>();
        services.AddSingleton<IDocumentIdManager>(provider => provider.GetRequiredService<DocumentIdManager>());
        services.AddSingleton<IActOnStartup>(provider => provider.GetRequiredService<DocumentIdManager>());
        services.AddSingleton<IDocumentManager, DocumentManager>();

        return services;
    }
}
