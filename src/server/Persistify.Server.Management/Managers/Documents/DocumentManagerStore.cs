using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Domain.Templates;
using Persistify.Server.Files;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Serialization;

namespace Persistify.Server.Management.Managers.Documents;

public class DocumentManagerStore : IDocumentManagerStore
{
    private readonly IAnalyzerExecutorFactory _analyzerExecutorFactory;
    private readonly IFileStreamFactory _fileStreamFactory;
    private readonly ConcurrentDictionary<int, IDocumentManager> _repositories;
    private readonly IOptions<RepositorySettings> _repositorySettingsOptions;
    private readonly ISerializer _serializer;
    private readonly ITransactionState _transactionState;
    private SpinLock _spinLock;

    public DocumentManagerStore(
        ITransactionState transactionState,
        IFileStreamFactory fileStreamFactory,
        ISerializer serializer,
        IOptions<RepositorySettings> repositorySettingsOptions,
        IAnalyzerExecutorFactory analyzerExecutorFactory
    )
    {
        _transactionState = transactionState;
        _fileStreamFactory = fileStreamFactory;
        _serializer = serializer;
        _repositorySettingsOptions = repositorySettingsOptions;
        _analyzerExecutorFactory = analyzerExecutorFactory;
        _repositories = new ConcurrentDictionary<int, IDocumentManager>();
        _spinLock = new SpinLock();
    }

    public IDocumentManager? GetManager(int templateId)
    {
        _repositories.TryGetValue(templateId, out var repository);
        return repository;
    }

    public void AddManager(Template template)
    {
        var lockTaken = false;
        try
        {
            _spinLock.Enter(ref lockTaken);

            if (_repositories.ContainsKey(template.Id))
            {
                throw new InvalidOperationException();
            }

            var repository = new DocumentManager(
                _transactionState,
                template,
                _fileStreamFactory,
                _serializer,
                _repositorySettingsOptions,
                _analyzerExecutorFactory
            );

            _repositories.TryAdd(template.Id, repository);
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

    public IEnumerable<IDocumentManager> GetManagers()
    {
        return _repositories.Values;
    }
}
