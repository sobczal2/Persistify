using System;
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
        
        var authSettingsSection = configuration
            .GetRequiredSection(AuthSettings.SectionName);
        var authSettingsValidator = new AuthSettingsValidator();
        authSettingsValidator.ValidateAndThrow(authSettingsSection.Get<AuthSettings>() ?? throw new InvalidOperationException($"Could not load {AuthSettings.SectionName} from configuration"));
        services.Configure<AuthSettings>(authSettingsSection);

        return services;
    }
}
