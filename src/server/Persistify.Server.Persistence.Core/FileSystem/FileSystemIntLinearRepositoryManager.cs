using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.Core.Exceptions;
using Persistify.Server.Persistence.Core.Stream;

namespace Persistify.Server.Persistence.Core.FileSystem;

public class FileSystemIntLinearRepositoryManager : IIntLinearRepositoryManager, IDisposable
{
    private readonly ConcurrentDictionary<string, IDisposable> _repositories;
    private readonly StorageSettings _storageSettings;

    public FileSystemIntLinearRepositoryManager(
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
        if (_repositories.ContainsKey(repositoryName))
        {
            throw new RepositoryAlreadyExistsException(repositoryName);
        }

        var parts = repositoryName.Split('/');
        var directoryPath = Path.Combine(_storageSettings.DataPath, Path.Combine(parts[..^1]));
        Directory.CreateDirectory(directoryPath);

        var filePath = Path.Combine(_storageSettings.DataPath, repositoryName);
        var mainFilePath = $"{filePath}.bin";

        _repositories.TryAdd(repositoryName, new StreamIntLinearRepository(
            new FileStream(mainFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)
        ));
    }

    public IIntLinearRepository Get(string repositoryName)
    {
        if (!_repositories.TryGetValue(repositoryName, out var repository))
        {
            throw new RepositoryNotFoundException(repositoryName);
        }

        return (IIntLinearRepository)repository;
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
