using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Configuration.Enums;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.HostedServices;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.Core.FileSystem;

namespace Persistify.Server.Persistence.Core;

public static class PersistenceCoreExtensions
{
    public static IServiceCollection AddPersistenceCore(this IServiceCollection services, IConfiguration configuration)
    {
        var storageSettings = configuration.GetSection(StorageSettings.SectionName).Get<StorageSettings>()!;

        switch (storageSettings.StorageType)
        {
            case StorageType.FileSystem:
                services.AddSingleton<FileSystemRepositoryManager>();
                services.AddSingleton<IRepositoryManager>(sp => sp.GetRequiredService<FileSystemRepositoryManager>());
                services.AddSingleton<IIntLinearRepositoryManager, FileSystemIntLinearRepositoryManager>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return services;
    }
}
