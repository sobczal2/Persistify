using Microsoft.Extensions.DependencyInjection;
using Persistify.HostedServices;
using Persistify.Management.Domain.Abstractions;

namespace Persistify.Management.Domain;

public static class ManagementDomainExtensions
{
    public static IServiceCollection AddManagementDomain(this IServiceCollection services)
    {
        services.AddSingleton<TemplateManager>();
        services.AddSingleton<ITemplateManager>(provider => provider.GetRequiredService<TemplateManager>());
        services.AddSingleton<IActOnStartup>(provider => provider.GetRequiredService<TemplateManager>());
        services.AddSingleton<IDocumentIdManager, DocumentIdManager>();

        return services;
    }
}
