using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Concurrency;
using Persistify.Domain.Templates;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.ErrorHandling;
using Persistify.Server.Management.Files;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Persistence.Object;
using Persistify.Server.Persistence.Primitives;
using Persistify.Server.Serialization;

namespace Persistify.Server.Management.Managers.Templates;

public class TemplateManager : Manager, ITemplateManager
{
    private readonly IDocumentManagerStore _documentManagerStore;
    private readonly IFileHandler _fileHandler;
    private readonly IntStreamRepository _identifierRepository;
    private readonly ConcurrentDictionary<string, int> _templateNameIdDictionary;
    private readonly ObjectStreamRepository<Template> _templateRepository;
    private volatile int _count;

    public TemplateManager(
        ITransactionState transactionState,
        IFileHandler fileHandler,
        IDocumentManagerStore documentManagerStore,
        IFileStreamFactory fileStreamFactory,
        ISerializer serializer,
        IOptions<RepositorySettings> repositorySettingsOptions
    ) : base(
        transactionState
    )
    {
        _fileHandler = fileHandler;
        _documentManagerStore = documentManagerStore;
        var identifierFileStream =
            fileStreamFactory.CreateStream(TemplateManagerRequiredFileGroup.IdentifierRepositoryFileName);
        var templateRepositoryMainFileStream =
            fileStreamFactory.CreateStream(TemplateManagerRequiredFileGroup.TemplateRepositoryMainFileName);
        var templateRepositoryOffsetLengthFileStream =
            fileStreamFactory.CreateStream(TemplateManagerRequiredFileGroup.TemplateRepositoryOffsetLengthFileName);

        _identifierRepository = new IntStreamRepository(identifierFileStream);
        _templateRepository = new ObjectStreamRepository<Template>(
            templateRepositoryMainFileStream,
            templateRepositoryOffsetLengthFileStream,
            serializer,
            repositorySettingsOptions.Value.TemplateRepositorySectorSize
        );

        _templateNameIdDictionary = new ConcurrentDictionary<string, int>();
        _count = 0;
    }

    public override string Name => "TemplateManager";

    public override void Initialize()
    {
        ThrowIfCannotWrite();

        var initializeAction = new Func<ValueTask>(async () =>
        {
            var currentId = await _identifierRepository.ReadAsync(0, true);

            if (_identifierRepository.IsValueEmpty(currentId))
            {
                await _identifierRepository.WriteAsync(0, 0, true);
            }

            _count = await _templateRepository.CountAsync(true);

            var read = 0;
            const int batchSize = 1000;

            while (read < _count)
            {
                var kvList = await _templateRepository.ReadRangeAsync(batchSize, read, true);

                foreach (var kv in kvList)
                {
                    _documentManagerStore.AddManager(kv.Key);
                    _templateNameIdDictionary.TryAdd(kv.Value.Name, kv.Key);
                }

                read += batchSize;
            }

            base.Initialize();
        });

        PendingActions.Enqueue(initializeAction);
    }

    public async ValueTask<Template?> GetAsync(string templateName)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        if (!_templateNameIdDictionary.TryGetValue(templateName, out var id))
        {
            return null;
        }

        return await _templateRepository.ReadAsync(id, true);
    }

    public async ValueTask<Template?> GetAsync(int id)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        return await _templateRepository.ReadAsync(id, true);
    }

    public bool Exists(string templateName)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        return _templateNameIdDictionary.ContainsKey(templateName);
    }

    public async ValueTask<List<Template>> ListAsync(int take, int skip)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        var kvList = await _templateRepository.ReadRangeAsync(take, skip, true);
        var list = new List<Template>(kvList.Count);

        foreach (var kv in kvList)
        {
            list.Add(kv.Value);
        }

        return list;
    }

    public int Count()
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        return _count;
    }

    public void Add(Template template)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotWrite();

        if (_templateNameIdDictionary.ContainsKey(template.Name))
        {
            throw new PersistifyInternalException();
        }

        var addAction = new Func<ValueTask>(async () =>
        {
            var currentId = await _identifierRepository.ReadAsync(0, true);

            currentId++;

            await _identifierRepository.WriteAsync(0, currentId, true);

            template.Id = currentId;

            await _templateRepository.WriteAsync(currentId, template, true);

            _fileHandler.CreateFilesForTemplate(currentId);
            _documentManagerStore.AddManager(currentId);

            var documentManager = _documentManagerStore.GetManager(currentId);

            if (documentManager is null)
            {
                throw new PersistifyInternalException();
            }

            await TransactionState.GetCurrentTransaction()
                .PromoteManagerAsync(documentManager, true, TransactionTimeout);

            documentManager.Initialize();

            _templateNameIdDictionary.TryAdd(template.Name, currentId);

            Interlocked.Increment(ref _count);
        });

        PendingActions.Enqueue(addAction);
    }

    public async ValueTask<bool> RemoveAsync(int id)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotWrite();

        var template = await _templateRepository.ReadAsync(id, true);

        if (template is null)
        {
            return false;
        }

        var removeAction = new Func<ValueTask>(async () =>
        {
            if (await _templateRepository.DeleteAsync(id, true))
            {
                var documentManager = _documentManagerStore.GetManager(id);
                if (documentManager is null)
                {
                    throw new PersistifyInternalException();
                }

                await TransactionState.GetCurrentTransaction()
                    .PromoteManagerAsync(documentManager, true, TransactionTimeout);

                _fileHandler.DeleteFilesForTemplate(id);
                _documentManagerStore.DeleteManager(id);

                _templateNameIdDictionary.TryRemove(template.Name, out _);

                Interlocked.Decrement(ref _count);
            }
            else
            {
                throw new PersistifyInternalException();
            }
        });

        PendingActions.Enqueue(removeAction);

        return true;
    }
}
