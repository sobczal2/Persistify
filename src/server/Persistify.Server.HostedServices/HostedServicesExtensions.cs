using Microsoft.Extensions.DependencyInjection;

namespace Persistify.Server.HostedServices;

public static class HostedServicesExtensions
{
    public static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<StartupActionHostedService>();

        return services;
    }
}
