using Microsoft.Extensions.DependencyInjection;
using Persistify.Core.Storage;

namespace Persistify.Storage.Sqlite;

public static class SqliteStorageExtensions
{
    public static IServiceCollection AddSqliteStorage(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IStorageProvider>(new SqliteStorageProvider(connectionString));

        return services;
    }
}