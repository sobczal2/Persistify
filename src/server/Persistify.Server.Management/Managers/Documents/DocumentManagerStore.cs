﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Management.Files;
using Persistify.Server.Serialization;

namespace Persistify.Server.Management.Managers.Documents;

public class DocumentManagerStore : IDocumentManagerStore
{
    private readonly IFileStreamFactory _fileStreamFactory;
    private readonly ISerializer _serializer;
    private readonly IOptions<RepositorySettings> _repositorySettingsOptions;
    private readonly ConcurrentDictionary<int, IDocumentManager> _repositories;
    private SpinLock _spinLock;

    public DocumentManagerStore(
        IFileStreamFactory fileStreamFactory,
        ISerializer serializer,
        IOptions<RepositorySettings> repositorySettingsOptions)
    {
        _fileStreamFactory = fileStreamFactory;
        _serializer = serializer;
        _repositorySettingsOptions = repositorySettingsOptions;
        _repositories = new ConcurrentDictionary<int, IDocumentManager>();
        _spinLock = new SpinLock();
    }

    public IDocumentManager? GetManager(int templateId)
    {
        _repositories.TryGetValue(templateId, out var repository);
        return repository;
    }

    public void AddManager(int templateId)
    {
        var lockTaken = false;
        try
        {
            _spinLock.Enter(ref lockTaken);

            if (_repositories.ContainsKey(templateId))
            {
                throw new InvalidOperationException();
            }

            var repository = new DocumentManager(
                templateId,
                _fileStreamFactory,
                _serializer,
                _repositorySettingsOptions
            );

            _repositories.TryAdd(templateId, repository);
        }
        finally
        {
            if (lockTaken)
            {
                _spinLock.Exit(false);
            }
        }
    }

    public void DeleteManager(int templateId)
    {
        var lockTaken = false;
        try
        {
            _spinLock.Enter(ref lockTaken);

            if (!_repositories.TryRemove(templateId, out _))
            {
                throw new KeyNotFoundException();
            }
        }
        finally
        {
            if (lockTaken)
            {
                _spinLock.Exit(false);
            }
        }
    }
}
