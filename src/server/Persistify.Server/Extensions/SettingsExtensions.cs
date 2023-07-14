using System;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Server.Extensions;

public static class SettingsExtensions
{
    public static IServiceCollection AddSettingsConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        var grpcSettingsSection = configuration
            .GetRequiredSection(GrpcSettings.SectionName);
        var grpcSettingsValidator = new GrpcSettingsValidator();
        grpcSettingsValidator.ValidateAndThrow(grpcSettingsSection.Get<GrpcSettings>() ??
                                               throw new InvalidOperationException(
                                                   $"Could not load {GrpcSettings.SectionName} from configuration"));
        services.Configure<GrpcSettings>(grpcSettingsSection);

        var authSettingsSection = configuration
            .GetRequiredSection(AuthSettings.SectionName);
        var authSettingsValidator = new AuthSettingsValidator();
        authSettingsValidator.ValidateAndThrow(authSettingsSection.Get<AuthSettings>() ??
                                               throw new InvalidOperationException(
                                                   $"Could not load {AuthSettings.SectionName} from configuration"));
        services.Configure<AuthSettings>(authSettingsSection);

        var storageSettingsSection = configuration
            .GetRequiredSection(StorageSettings.SectionName);
        var storageSettingsValidator = new StorageSettingsValidator();
        storageSettingsValidator.ValidateAndThrow(storageSettingsSection.Get<StorageSettings>() ??
                                                  throw new InvalidOperationException(
                                                      $"Could not load {StorageSettings.SectionName} from configuration"));
        services.Configure<StorageSettings>(storageSettingsSection);

        var loggingSettingsSection = configuration
            .GetRequiredSection(LoggingSettings.SectionName);

        var loggingSettingsValidator = new LoggingSettingsValidator();
        loggingSettingsValidator.ValidateAndThrow(loggingSettingsSection.Get<LoggingSettings>() ??
                                                  throw new InvalidOperationException(
                                                      $"Could not load {LoggingSettings.SectionName} from configuration"));

        services.Configure<LoggingSettings>(loggingSettingsSection);

        var hostedServicesSettingsSection = configuration
            .GetRequiredSection(HostedServicesSettings.SectionName);

        var hostedServicesSettingsValidator = new HostedServicesSettingsValidator();
        hostedServicesSettingsValidator.ValidateAndThrow(hostedServicesSettingsSection.Get<HostedServicesSettings>() ??
                                                         throw new InvalidOperationException(
                                                             $"Could not load {HostedServicesSettings.SectionName} from configuration"));

        services.Configure<HostedServicesSettings>(hostedServicesSettingsSection);

        var cacheSettingsSection = configuration
            .GetRequiredSection(CacheSettings.SectionName);

        var cacheSettingsValidator = new CacheSettingsValidator();
        cacheSettingsValidator.ValidateAndThrow(cacheSettingsSection.Get<CacheSettings>() ??
                                                throw new InvalidOperationException(
                                                    $"Could not load {CacheSettings.SectionName} from configuration"));

        services.Configure<CacheSettings>(cacheSettingsSection);

        return services;
    }
}
