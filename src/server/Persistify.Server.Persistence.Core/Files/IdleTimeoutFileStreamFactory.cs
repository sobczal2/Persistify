using System;
using System.IO;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Persistence.LowLevel.Streams;

namespace Persistify.Server.Persistence.Core.Files;

public class IdleTimeoutFileStreamFactory : IFileStreamFactory
{
    private readonly StorageSettings _storageSettings;

    public IdleTimeoutFileStreamFactory(
        IOptions<StorageSettings> storageSettingsOptions
        )
    {
        _storageSettings = storageSettingsOptions.Value;
    }
    public Stream CreateStream(string relativePath)
    {
        var filePath = Path.Combine(_storageSettings.DataPath, relativePath);

        return new IdleTimeoutFileStream(
            _storageSettings.IdleFileTimeout,
            filePath
        );
    }
}
