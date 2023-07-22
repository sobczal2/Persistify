using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Persistence.Core.Abstractions;
using Persistify.Persistence.Core.FileSystem;
using Persistify.Persistence.Core.Sqlite;
using Persistify.Server.Configuration.Enums;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Persistence.Core;

public static class PersistenceCoreExtensions
{
    public static IServiceCollection AddPersistenceCore(this IServiceCollection services, IConfiguration configuration)
    {
        var storageSettings = configuration.GetSection(StorageSettings.SectionName).Get<StorageSettings>()!;

        switch (storageSettings.StorageType)
        {
            case StorageType.PrimitiveFileSystem:
                services.AddSingleton<IRepositoryFactory, PrimitiveFileSystemRepositoryFactory>();
                break;
            case StorageType.BinaryFileSystem:
                throw new NotImplementedException();
            case StorageType.Sqlite:
                services.AddSingleton<IRepositoryFactory, SqliteRepositoryFactory>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return services;
    }
}
