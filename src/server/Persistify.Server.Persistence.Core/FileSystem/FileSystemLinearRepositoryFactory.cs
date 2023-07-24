using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Persistence.Core.FileSystem;

public class FileSystemLinearRepositoryFactory : ILinearRepositoryFactory, IDisposable
{
    private readonly ConcurrentDictionary<string, IDisposable> _repositories;
    private readonly StorageSettings _storageSettings;

    public FileSystemLinearRepositoryFactory(
        IOptions<StorageSettings> storageSettings
    )
    {
        _storageSettings = storageSettings.Value;
        _repositories = new ConcurrentDictionary<string, IDisposable>();
    }

    public void Dispose()
    {
        foreach (var repository in _repositories.Values)
        {
            repository.Dispose();
        }
    }

    public ILongLinearRepository CreateLong(string repositoryName)
    {
        var filePath = Path.Combine(_storageSettings.DataPath, repositoryName);
        var mainFilePath = $"{filePath}.bin";
        return (ILongLinearRepository)_repositories.GetOrAdd(repositoryName,
            static (_, mainFilePath)
                => new FileSystemLongLinearRepository(mainFilePath),
            mainFilePath);
    }
}
