using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.Core.Exceptions;
using Persistify.Server.Persistence.Core.Stream;

namespace Persistify.Server.Persistence.Core.FileSystem;

public class FileSystemLongLinearRepositoryManager : ILongLinearRepositoryManager, IDisposable
{
    private readonly ConcurrentDictionary<string, IDisposable> _repositories;
    private readonly StorageSettings _storageSettings;

    public FileSystemLongLinearRepositoryManager(
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

    public void Create(string repositoryName)
    {
        var parts = repositoryName.Split('/');
        var directoryPath = Path.Combine(_storageSettings.DataPath, Path.Combine(parts[..^1]));
        Directory.CreateDirectory(directoryPath);

        var filePath = Path.Combine(_storageSettings.DataPath, repositoryName);
        var mainFilePath = $"{filePath}.bin";

        if (!_repositories.TryAdd(repositoryName, new StreamLongLinearRepository(
                new FileStream(mainFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)
            )))
        {
            throw new RepositoryAlreadyExistsException(repositoryName);
        }
    }

    public ILongLinearRepository Get(string repositoryName)
    {
        if (!_repositories.TryGetValue(repositoryName, out var repository))
        {
            throw new RepositoryNotFoundException(repositoryName);
        }

        return (ILongLinearRepository)repository;
    }

    public void Delete(string repositoryName)
    {
        if (!_repositories.TryRemove(repositoryName, out var repository))
        {
            throw new RepositoryNotFoundException(repositoryName);
        }

        repository.Dispose();

        var filePath = Path.Combine(_storageSettings.DataPath, repositoryName);
        var mainFilePath = $"{filePath}.bin";

        File.Delete(mainFilePath);
    }
}
