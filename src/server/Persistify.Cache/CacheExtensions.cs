using Microsoft.Extensions.DependencyInjection;

namespace Persistify.Cache;

public static class CacheExtensions
{
    public static IServiceCollection AddCache<TAbstract, TImpl>(this IServiceCollection services)
        where TAbstract : class
        where TImpl : class, TAbstract
    {
        services.AddSingleton<TAbstract, TImpl>();
        services.AddSingleton<ICache>(provider => provider.GetRequiredService<TAbstract>() as ICache);

        return services;
    }
}
