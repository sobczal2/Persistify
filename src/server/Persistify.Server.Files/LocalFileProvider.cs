﻿using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Server.Files;

public class LocalFileProvider : IFileProvider
{
    private readonly ILogger<LocalFileProvider> _logger;
    private readonly StorageSettings _storageSettings;

    public LocalFileProvider(
        ILogger<LocalFileProvider> logger,
        IOptions<StorageSettings> storageSettingsOptions
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ArgumentNullException.ThrowIfNull(storageSettingsOptions, nameof(storageSettingsOptions));
        _storageSettings = storageSettingsOptions.Value;

        if (!Directory.Exists(_storageSettings.DataPath))
        {
            Directory.CreateDirectory(_storageSettings.DataPath);
        }
    }

    public bool Exists(
        string relativePath
    )
    {
        var absolutePath = GetAbsolutePath(relativePath);
        _logger.LogTrace("Checking if file {AbsolutePath} exists", absolutePath);
        return File.Exists(absolutePath);
    }

    public void Create(
        string relativePath
    )
    {
        var absolutePath = GetAbsolutePath(relativePath);
        _logger.LogTrace("Creating file {AbsolutePath}", absolutePath);
        if (File.Exists(absolutePath))
        {
            throw new InvalidOperationException($"File {absolutePath} already exists");
        }

        if (!Directory.Exists(Path.GetDirectoryName(absolutePath)))
        {
            Directory.CreateDirectory(
                Path.GetDirectoryName(absolutePath)
                ?? throw new InvalidOperationException(
                    $"Could not get directory name from {absolutePath}"
                )
            );
        }

        File.Create(absolutePath).Dispose();
    }

    public void Delete(
        string relativePath
    )
    {
        var absolutePath = GetAbsolutePath(relativePath);
        _logger.LogTrace("Deleting file {AbsolutePath}", absolutePath);
        if (!File.Exists(absolutePath))
        {
            throw new InvalidOperationException($"File {absolutePath} does not exist");
        }

        File.Delete(absolutePath);
    }

    private string GetAbsolutePath(
        string relativePath
    )
    {
        return Path.Combine(_storageSettings.DataPath, relativePath);
    }
}
