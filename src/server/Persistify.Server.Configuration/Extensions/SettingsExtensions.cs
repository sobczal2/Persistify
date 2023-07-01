using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Server.Configuration.Extensions;

public static class SettingsExtensions
{
    public static IServiceCollection AddSettingsConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        var grpcSettingsSection = configuration
            .GetRequiredSection(GrpcSettings.SectionName);
        var grpcSettingsValidator = new GrpcSettingsValidator();
        grpcSettingsValidator.ValidateAndThrow(grpcSettingsSection.Get<GrpcSettings>() ?? throw new InvalidOperationException($"Could not load {GrpcSettings.SectionName} from configuration"));
        services.Configure<GrpcSettings>(grpcSettingsSection);
        
        
        return services;
    }
}