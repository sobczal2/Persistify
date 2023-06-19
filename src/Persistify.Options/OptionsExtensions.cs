using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Options.Storage;

namespace Persistify.Options;

public static class OptionsExtensions
{
    public static IServiceCollection AddAndValidateOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SecurityOptions>()
            .Bind(configuration.GetSection(SecurityOptions.SectionName))
            .Validate(x =>
            {
                new SecurityOptionsValidator().ValidateAndThrow(x);
                return true;
            });
        
        services.AddOptions<StorageOptions>()
            .Bind(configuration.GetSection(StorageOptions.SectionName))
            .Validate(x =>
            {
                new StorageOptionsValidator().ValidateAndThrow(x);
                return true;
            });

        return services;
    }
}