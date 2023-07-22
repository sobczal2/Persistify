﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.HostedServices;
using Persistify.Persistence.Core.Abstractions;
using Persistify.Serialization;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Persistence.Core.FileSystem;

public class FileSystemRepositoryFactory : IRepositoryFactory, IActRecurrently, IDisposable
{
    private readonly ConcurrentDictionary<string, IDisposable> _repositories;
    private readonly ISerializer _serializer;
    private readonly StorageSettings _storageSettings;

    public FileSystemRepositoryFactory(
        IOptions<StorageSettings> storageSettings,
        ISerializer serializer
    )
    {
        _storageSettings = storageSettings.Value;
        _serializer = serializer;
        _repositories = new ConcurrentDictionary<string, IDisposable>();
    }

    public IRepository<T> Create<T>(string repositoryName)
    {
        var baseFilesPath = Path.Combine(_storageSettings.DataPath, repositoryName);
        var mainFilePath = $"{baseFilesPath}.bin";
        var offsetsFilePath = $"{baseFilesPath}.offsets.bin";
        var lengthsFilePath = $"{baseFilesPath}.lengths.bin";
        var offsetsRepository = new FileSystemLongLinearRepository(offsetsFilePath);
        var lengthsRepository = new FileSystemLongLinearRepository(lengthsFilePath);

        return (IRepository<T>)_repositories.GetOrAdd(repositoryName,
            static (_, args)
                => new FileSystemRepository<T>(args.mainFilePath, args.offsetsRepository, args.lengthsRepository,
                    args.serializer),
            (serializer: _serializer, mainFilePath: mainFilePath, offsetsRepository: offsetsRepository,
                lengthsRepository: lengthsRepository));
    }

    public TimeSpan RecurrentActionInterval => TimeSpan.FromSeconds(30);
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
}
