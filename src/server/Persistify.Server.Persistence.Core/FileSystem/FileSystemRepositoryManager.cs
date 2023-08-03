using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.Core.Exceptions;
using Persistify.Server.Persistence.Core.Stream;
using Persistify.Server.Serialization;

namespace Persistify.Server.Persistence.Core.FileSystem;

public class FileSystemRepositoryManager : IRepositoryManager, IDisposable
{
    private readonly ILongLinearRepositoryManager _longLinearRepositoryManager;
    private readonly ConcurrentDictionary<string, IDisposable> _repositories;
    private readonly ISerializer _serializer;
    private readonly StorageSettings _storageSettings;

    public FileSystemRepositoryManager(
        IOptions<StorageSettings> storageSettings,
        ISerializer serializer,
        ILongLinearRepositoryManager longLinearRepositoryManager
    )
    {
        _storageSettings = storageSettings.Value;
        _serializer = serializer;
        _longLinearRepositoryManager = longLinearRepositoryManager;
        _repositories = new ConcurrentDictionary<string, IDisposable>();
    }

    public void Dispose()
    {
        foreach (var repository in _repositories.Values)
        {
            repository.Dispose();
        }
    }

    public void Create<T>(string repositoryName) where T : class
    {
        if (_repositories.ContainsKey(repositoryName))
        {
            throw new RepositoryAlreadyExistsException(repositoryName);
        }

        var parts = repositoryName.Split('/');
        var directoryPath = Path.Combine(_storageSettings.DataPath, Path.Combine(parts[..^1]));
        Directory.CreateDirectory(directoryPath);

        var baseFilesPath = Path.Combine(_storageSettings.DataPath, repositoryName);
        var mainFilePath = $"{baseFilesPath}.bin";
        var indexRepositoryName = $"{repositoryName}.idx";

        _longLinearRepositoryManager.Create(indexRepositoryName);

        if (!_repositories.TryAdd(repositoryName, new StreamRepository<T>(
                _longLinearRepositoryManager.Get(indexRepositoryName),
                _serializer,
                new FileStream(mainFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None),
                _storageSettings.RepositorySectorSize
            )))
        {
            throw new RepositoryAlreadyExistsException(repositoryName);
        }
    }

    public IRepository<T> Get<T>(string repositoryName) where T : class
    {
        if (!_repositories.TryGetValue(repositoryName, out var repository))
        {
            throw new RepositoryNotFoundException(repositoryName);
        }

        return (IRepository<T>)repository;
    }

    public bool Exists<T>(string repositoryName) where T : class
    {
        return _repositories.ContainsKey(repositoryName);
    }

    public void Delete<T>(string repositoryName) where T : class
    {
        if (!_repositories.TryRemove(repositoryName, out var repository))
        {
            throw new RepositoryNotFoundException(repositoryName);
        }

        repository.Dispose();

        var baseFilesPath = Path.Combine(_storageSettings.DataPath, repositoryName);
        var mainFilePath = $"{baseFilesPath}.bin";
        var indexRepositoryName = $"{repositoryName}.idx";

        File.Delete(mainFilePath);
        _longLinearRepositoryManager.Delete(indexRepositoryName);
    }
}
