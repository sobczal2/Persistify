using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Persistify.Options;

public static class OptionsExtensions
{
    public static IServiceCollection AddPersistifyOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AuthOptions>()
            .Bind(configuration.GetSection(AuthOptions.SectionName))
            .ValidateDataAnnotations();
        
        services.AddOptions<PersistifyOptions>()
            .Bind(configuration.GetSection(PersistifyOptions.SectionName))
            .ValidateDataAnnotations();

        return services;
    }
}