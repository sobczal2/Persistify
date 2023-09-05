using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.HostedServices.Actions;
using Persistify.Server.HostedServices.Implementations;

namespace Persistify.Server.HostedServices;

public static class HostedServicesExtensions
{
    public static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddStartupActions();

        services.AddHostedService<RecurrentServicesHostedService>();
        services.AddHostedService<StartupActionHostedService>();

        return services;
    }

    private static IServiceCollection AddStartupActions(this IServiceCollection services)
    {
        services.AddTransient<IStartupAction, ManagementStartupAction>();

        return services;
    }
}
