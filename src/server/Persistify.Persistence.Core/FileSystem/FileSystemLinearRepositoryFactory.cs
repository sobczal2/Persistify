using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Persistify.Persistence.Core.Abstractions;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Persistence.Core.FileSystem;

public class FileSystemLinearRepositoryFactory : ILinearRepositoryFactory
{
    private readonly ConcurrentDictionary<string, object> _repositories;
    private readonly StorageSettings _storageSettings;

    public FileSystemLinearRepositoryFactory(
        IOptions<StorageSettings> storageSettings
    )
    {
        _storageSettings = storageSettings.Value;
    }

    public ILongLinearRepository CreateLong(string repositoryName)
    {
        var directoryPath = Path.Combine(_storageSettings.DataPath, repositoryName);
        return (ILongLinearRepository)_repositories.GetOrAdd(repositoryName,
            static (_, directoryPath)
                => new FileSystemLongLinearRepository(directoryPath),
            directoryPath);
    }
}
