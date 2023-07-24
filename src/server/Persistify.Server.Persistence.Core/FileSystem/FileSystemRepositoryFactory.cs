using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.HostedServices;
using Persistify.Server.HostedServices.Abstractions;
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
        return (IRepository<T>)_repositories.GetOrAdd(repositoryName,
            static (_, args)
                =>
            {
                var parts = args.repositoryName.Split('/');
                var directoryPath = Path.Combine(args.dataPath, Path.Combine(parts[..^1]));
                Directory.CreateDirectory(directoryPath);

                var baseFilesPath = Path.Combine(args.dataPath, args.repositoryName);
                var mainFilePath = $"{baseFilesPath}.bin";

                var offsetsRepositoryName = $"{args.repositoryName}.offsets";
                var lengthsRepositoryName = $"{args.repositoryName}.lengths";

                var offsetsRepository = args.linearRepositoryFactory.CreateLong(offsetsRepositoryName);
                var lengthsRepository = args.linearRepositoryFactory.CreateLong(lengthsRepositoryName);

                return new FileSystemRepository<T>(mainFilePath, offsetsRepository, lengthsRepository,
                    args.serializer);
            },
            (serializer: _serializer, repositoryName, dataPath: _storageSettings.DataPath,
                linearRepositoryFactory: _linearRepositoryFactory));
    }
}
