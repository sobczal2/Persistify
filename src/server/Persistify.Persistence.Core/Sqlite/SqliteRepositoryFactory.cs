using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Persistify.Persistence.Core.Abstractions;
using Persistify.Serialization;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Persistence.Core.Sqlite;

public class SqliteRepositoryFactory : IRepositoryFactory
{
    private readonly ConcurrentDictionary<string, object> _repositories;
    private readonly ISerializer _serializer;
    private readonly IOptions<StorageSettings> _storageSettings;

    public SqliteRepositoryFactory(
        IOptions<StorageSettings> storageSettings,
        ISerializer serializer
    )
    {
        _storageSettings = storageSettings;
        _serializer = serializer;
        _repositories = new ConcurrentDictionary<string, object>();
    }

    public IRepository<T> Create<T>(string repositoryName)
    {
        var connectionString = $"Data Source={_storageSettings.Value.DataPath}/.db";
        return (IRepository<T>)_repositories.GetOrAdd(repositoryName, (key, args) =>
        {
            var tableName = $"{key}";
            return new SqliteRepository<T>(args.connectionString, tableName, args.serializer);
        }, (serializer: _serializer, connectionString));
    }
}
