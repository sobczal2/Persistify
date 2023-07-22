using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Persistify.Persistence.Core.Abstractions;
using Persistify.Serialization;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Persistence.Core.FileSystem;

public class PrimitiveFileSystemRepositoryFactory : IRepositoryFactory
{
    private readonly ConcurrentDictionary<string, object> _repositories;
    private readonly ISerializer _serializer;
    private readonly StorageSettings _storageSettings;

    public PrimitiveFileSystemRepositoryFactory(
        IOptions<StorageSettings> storageSettings,
        ISerializer serializer
    )
    {
        _storageSettings = storageSettings.Value;
        _serializer = serializer;
        _repositories = new ConcurrentDictionary<string, object>();
    }

    public IRepository<T> Create<T>(string repositoryName)
    {
        var directoryPath = Path.Combine(_storageSettings.DataPath, repositoryName);
        return (IRepository<T>)_repositories.GetOrAdd(repositoryName,
            static (_, args)
                => new PrimitiveFileSystemRepository<T>(args.directoryPath, args.serializer),
            (serializer: _serializer, directoryPath));
    }
}
