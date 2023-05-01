using Microsoft.Extensions.DependencyInjection;

namespace Persistify.Client;

public static class PersistifyClientExtensions
{
    public static IServiceCollection AddPersistifyClient(this IServiceCollection services, Action<PersistifyClientOptions> configure)
    {
        var options = new PersistifyClientOptions();
        configure(options);
        
        services.AddSingleton(options);
        services.AddSingleton<IPersistifyClient, PersistifyClient>();
        
        return services;
    }
}