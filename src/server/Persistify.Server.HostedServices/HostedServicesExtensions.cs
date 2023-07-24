using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Persistify.Server.HostedServices;

public static class HostedServicesExtensions
{
    public static IServiceCollection AddHostedServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddActOnStartupServices(assemblies);
        services.AddActOnShutdownServices(assemblies);
        services.AddActRecurrentlyServices(assemblies);

        services.AddHostedService<StartupServicesHostedService>();
        services.AddHostedService<ShutdownServicesHostedService>();
        services.AddHostedService<RecurrentServicesHostedService>();

        return services;
    }

    private static IServiceCollection AddActOnStartupServices(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var actOnStartupServices = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IActOnStartup).IsAssignableFrom(type) && type.IsInterface &&
                           type != typeof(IActOnStartup))
            .ToList();

        foreach (var actOnStartupService in actOnStartupServices)
        {
            services.AddSingleton(typeof(IActOnStartup), s => s.GetRequiredService(actOnStartupService));
        }

        return services;
    }

    private static IServiceCollection AddActOnShutdownServices(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var actOnShutdownServices = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IActOnShutdown).IsAssignableFrom(type) && type.IsInterface &&
                           type != typeof(IActOnShutdown))
            .ToList();

        foreach (var actOnShutdownService in actOnShutdownServices)
        {
            services.AddSingleton(typeof(IActOnShutdown), s => s.GetRequiredService(actOnShutdownService));
        }

        return services;
    }

    private static IServiceCollection AddActRecurrentlyServices(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var actRecurrentlyServices = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IActRecurrently).IsAssignableFrom(type) && type.IsInterface &&
                           type != typeof(IActRecurrently))
            .ToList();

        foreach (var actRecurrentlyService in actRecurrentlyServices)
        {
            services.AddSingleton(typeof(IActRecurrently), s => s.GetRequiredService(actRecurrentlyService));
        }

        return services;
    }
}
