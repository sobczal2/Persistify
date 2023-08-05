using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.Core.Exceptions;
using Persistify.Server.Persistence.Core.Stream;
using Persistify.Server.Persistence.Core.Transactions;
using Persistify.Server.Persistence.Core.Transactions.RepositoryWrappers;

namespace Persistify.Server.Persistence.Core.FileSystem;

public class FileSystemLongLinearRepositoryManager : ILongLinearRepositoryManager, IDisposable
{
    private readonly ISystemClock _systemClock;
    private readonly ConcurrentDictionary<string, IDisposable> _repositories;
    private readonly StorageSettings _storageSettings;

    public FileSystemLongLinearRepositoryManager(
        IOptions<StorageSettings> storageSettings,
        ISystemClock systemClock
    )
    {
        _systemClock = systemClock;
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

        if (TransactionState.Current is not null)
        {
            return new LongLinearRepositoryTransactionWrapper(
                (ILongLinearRepository)repository,
                TransactionState.RequiredCurrent,
                _systemClock
            );
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
