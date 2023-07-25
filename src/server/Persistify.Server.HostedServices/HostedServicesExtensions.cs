﻿using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.HostedServices.Implementations;

namespace Persistify.Server.HostedServices;

public static class HostedServicesExtensions
{
    public static IServiceCollection AddHostedServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddActOnStartupServices(assemblies);
        services.AddActRecurrentlyServices(assemblies);

        services.AddHostedService<StartupServicesHostedService>();
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