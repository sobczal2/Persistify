using Microsoft.Extensions.DependencyInjection;
using Persistify.Helpers.Time;

namespace Persistify.Helpers;

public static class HelpersExtensions
{
    public static IServiceCollection AddHelpers(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}
