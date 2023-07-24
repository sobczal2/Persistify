using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.HostedServices;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Serialization;

namespace Persistify.Server.Persistence.Core.FileSystem;

public class FileSystemRepositoryFactory : IRepositoryFactory, IActRecurrently, IDisposable
{
    private readonly ILinearRepositoryFactory _linearRepositoryFactory;
    private readonly ConcurrentDictionary<string, IDisposable> _repositories;
    private readonly ISerializer _serializer;
    private readonly StorageSettings _storageSettings;

    public FileSystemRepositoryFactory(
        IOptions<StorageSettings> storageSettings,
        ISerializer serializer,
        ILinearRepositoryFactory linearRepositoryFactory
    )
    {
        _storageSettings = storageSettings.Value;
        _serializer = serializer;
        _linearRepositoryFactory = linearRepositoryFactory;
        _repositories = new ConcurrentDictionary<string, IDisposable>();
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

    public IRepository<T> Create<T>(string repositoryName)
    {
        var baseFilesPath = Path.Combine(_storageSettings.DataPath, repositoryName);
        var mainFilePath = $"{baseFilesPath}.bin";
        var offsetsFilePath = $"{baseFilesPath}.offsets.bin";
        var lengthsFilePath = $"{baseFilesPath}.lengths.bin";
        var offsetsRepository = _linearRepositoryFactory.CreateLong(offsetsFilePath);
        var lengthsRepository = _linearRepositoryFactory.CreateLong(lengthsFilePath);

        return (IRepository<T>)_repositories.GetOrAdd(repositoryName,
            static (_, args)
                => new FileSystemRepository<T>(args.mainFilePath, args.offsetsRepository, args.lengthsRepository,
                    args.serializer),
            (serializer: _serializer, mainFilePath, offsetsRepository,
                lengthsRepository));
    }
}
