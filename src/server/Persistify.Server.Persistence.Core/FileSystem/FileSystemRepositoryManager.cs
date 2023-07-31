using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.Core.Exceptions;
using Persistify.Server.Serialization;

namespace Persistify.Server.Persistence.Core.FileSystem;

public class FileSystemRepositoryManager : IRepositoryManager, IActRecurrently, IDisposable
{
    private readonly IIntLinearRepositoryManager _intLinearRepositoryManager;
    private readonly ConcurrentDictionary<string, IDisposable> _repositories;
    private readonly ISerializer _serializer;
    private readonly StorageSettings _storageSettings;
    private readonly object _lock;

    public FileSystemRepositoryManager(
        IOptions<StorageSettings> storageSettings,
        ISerializer serializer,
        IIntLinearRepositoryManager intLinearRepositoryManager
    )
    {
        _storageSettings = storageSettings.Value;
        _serializer = serializer;
        _intLinearRepositoryManager = intLinearRepositoryManager;
        _repositories = new ConcurrentDictionary<string, IDisposable>();
        _lock = new object();
    }

    public TimeSpan RecurrentActionInterval => TimeSpan.FromMinutes(15);

    public async ValueTask PerformRecurrentActionAsync()
    {
        foreach (var repository in _repositories.Values)
        {
            await ((IPurgable)repository).PurgeAsync();
        }

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        foreach (var repository in _repositories.Values)
        {
            repository.Dispose();
        }
    }

    public void Create<T>(string repositoryName)
    {
        lock (_lock)
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

            var offsetsRepositoryName = $"{repositoryName}.offsets";
            var lengthsRepositoryName = $"{repositoryName}.lengths";

            _intLinearRepositoryManager.Create(offsetsRepositoryName);
            _intLinearRepositoryManager.Create(lengthsRepositoryName);
            var offsetsRepository = _intLinearRepositoryManager.Get(offsetsRepositoryName);
            var lengthsRepository = _intLinearRepositoryManager.Get(lengthsRepositoryName);

            if(!_repositories.TryAdd(repositoryName, new FileSystemRepository<T>(mainFilePath, offsetsRepository, lengthsRepository, _serializer)))
            {
                throw new RepositoryFactoryInternalException();
            }
        }
    }

    public IRepository<T> Get<T>(string repositoryName)
    {
        if (!_repositories.TryGetValue(repositoryName, out var repository))
        {
            throw new RepositoryNotFoundException(repositoryName);
        }

        return (IRepository<T>)repository;
    }

    public bool Exists<T>(string repositoryName)
    {
        return _repositories.ContainsKey(repositoryName);
    }

    public void Delete<T>(string repositoryName)
    {
        if (!_repositories.TryRemove(repositoryName, out var repository))
        {
            throw new RepositoryNotFoundException(repositoryName);
        }

        repository.Dispose();
    }
}
