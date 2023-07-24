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
        return (ILongLinearRepository)_repositories.GetOrAdd(repositoryName,
            static (_, args)
                =>
            {
                var parts = args.repositoryName.Split('/');
                var directoryPath = Path.Combine(args.dataPath, Path.Combine(parts[..^1]));
                Directory.CreateDirectory(directoryPath);

                var filePath = Path.Combine(args.dataPath, args.repositoryName);
                var mainFilePath = $"{filePath}.bin";

                return new FileSystemLongLinearRepository(mainFilePath);
            },
            (dataPath: _storageSettings.DataPath, repositoryName));
    }
}
