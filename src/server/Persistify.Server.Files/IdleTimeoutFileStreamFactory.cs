using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Persistence.Streams;

namespace Persistify.Server.Files;

public class IdleTimeoutFileStreamFactory : IFileStreamFactory
{
    private readonly ILogger<IdleTimeoutFileStreamFactory> _logger;
    private readonly StorageSettings _storageSettings;

    public IdleTimeoutFileStreamFactory(
        IOptions<StorageSettings> storageSettingsOptions,
        ILogger<IdleTimeoutFileStreamFactory> logger
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ArgumentNullException.ThrowIfNull(storageSettingsOptions, nameof(storageSettingsOptions));
        _storageSettings = storageSettingsOptions.Value;
    }

    public Stream CreateStream(string relativePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(relativePath, nameof(relativePath));

        var filePath = Path.Combine(_storageSettings.DataPath, relativePath);

        _logger.LogTrace("Creating IdleTimeoutFileStream for {FilePath}", filePath);

        return new IdleTimeoutFileStream(_storageSettings.IdleFileTimeout, filePath);
    }
}
