using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Domain.Documents;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Management.Files;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Persistence.Object;
using Persistify.Server.Persistence.Primitives;
using Persistify.Server.Serialization;

namespace Persistify.Server.Management.Managers.Documents;

public class DocumentManager : Manager, IDocumentManager
{
    private readonly int _templateId;
    private readonly IntStreamRepository _identifierRepository;
    private readonly ObjectStreamRepository<Document> _documentRepository;
    private volatile int _count;

    public DocumentManager(
        ITransactionState transactionState,
        int templateId,
        IFileStreamFactory fileStreamFactory,
        ISerializer serializer,
        IOptions<RepositorySettings> repositorySettingsOptions
    ) : base(
        transactionState
    )
    {
        _templateId = templateId;

        var identifierFileStream =
            fileStreamFactory.CreateStream(DocumentManagerFileGroupForTemplate.IdentifierFileName(_templateId));
        var documentRepositoryMainFileStream =
            fileStreamFactory.CreateStream(
                DocumentManagerFileGroupForTemplate.DocumentRepositoryMainFileName(_templateId));
        var documentRepositoryOffsetLengthFileStream =
            fileStreamFactory.CreateStream(
                DocumentManagerFileGroupForTemplate.DocumentRepositoryOffsetLengthFileName(_templateId));

        _identifierRepository = new IntStreamRepository(identifierFileStream);
        _documentRepository = new ObjectStreamRepository<Document>(
            documentRepositoryMainFileStream,
            documentRepositoryOffsetLengthFileStream,
            serializer,
            repositorySettingsOptions.Value.DocumentRepositorySectorSize
        );

        _count = 0;
    }

    public override string Name => $"DocumentManager_{_templateId:x8}";

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

            _count = await _documentRepository.CountAsync(true);

            base.Initialize();
        });

        PendingActions.Enqueue(initializeAction);
    }

    public async ValueTask<Document?> GetAsync(int id)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        return await _documentRepository.ReadAsync(id, true);
    }

    public async ValueTask<List<Document>> ListAsync(int take, int skip)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        var kvList = await _documentRepository.ReadRangeAsync(take, skip, true);
        var list = new List<Document>(kvList.Count);

        foreach (var (key, value) in kvList)
        {
            list.Add(value);
        }

        return list;
    }

    public int Count()
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        return _count;
    }

    public void Add(Document document)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotWrite();

        var addAction = new Func<ValueTask>(async () =>
        {
            var currentId = await _identifierRepository.ReadAsync(0, true);

            currentId++;

            await _identifierRepository.WriteAsync(0, currentId, true);

            await _documentRepository.WriteAsync(currentId, document, true);

            document.Id = currentId;
        });

        PendingActions.Enqueue(addAction);
    }

    public async ValueTask<bool> RemoveAsync(int id)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotWrite();

        if (!await _documentRepository.ExistsAsync(id, true))
        {
            return false;
        }

        var removeAction = new Func<ValueTask>(async () =>
        {
            await _documentRepository.DeleteAsync(id, true);
        });

        PendingActions.Enqueue(removeAction);

        return true;
    }
}
