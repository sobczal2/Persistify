using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Dtos.Documents.Search;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Domain.Documents;
using Persistify.Server.Domain.Templates;
using Persistify.Server.Files;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Indexes.Indexers.Common;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Mappers.Documents;
using Persistify.Server.Persistence.Extensions;
using Persistify.Server.Persistence.Object;
using Persistify.Server.Persistence.Primitives;
using Persistify.Server.Serialization;

namespace Persistify.Server.Management.Managers.Documents;

public class DocumentManager : Manager, IDocumentManager
{
    private readonly ObjectStreamRepository<Document> _documentRepository;
    private readonly IntStreamRepository _identifierRepository;
    private readonly IndexerStore _indexerStore;
    private readonly Template _template;
    private volatile int _count;

    public DocumentManager(
        ITransactionState transactionState,
        Template template,
        IFileStreamFactory fileStreamFactory,
        ISerializer serializer,
        IOptions<RepositorySettings> repositorySettingsOptions,
        IAnalyzerExecutorFactory analyzerExecutorFactory
    ) : base(
        transactionState
    )
    {
        _template = template;

        _indexerStore = new IndexerStore(template, analyzerExecutorFactory);

        var identifierFileStream =
            fileStreamFactory.CreateStream(DocumentManagerFileGroupForTemplate.IdentifierFileName(_template.Id));
        var documentRepositoryMainFileStream =
            fileStreamFactory.CreateStream(
                DocumentManagerFileGroupForTemplate.DocumentRepositoryMainFileName(_template.Id));
        var documentRepositoryOffsetLengthFileStream =
            fileStreamFactory.CreateStream(
                DocumentManagerFileGroupForTemplate.DocumentRepositoryOffsetLengthFileName(_template.Id));

        _identifierRepository = new IntStreamRepository(identifierFileStream);
        _documentRepository = new ObjectStreamRepository<Document>(
            documentRepositoryMainFileStream,
            documentRepositoryOffsetLengthFileStream,
            serializer,
            repositorySettingsOptions.Value.DocumentRepositorySectorSize
        );

        _count = 0;
    }

    public override string Name => $"DocumentManager_{_template.Id:x8}";

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

            _count = 0;

            await foreach (var (_, value) in _documentRepository.ReadAllAsync(true))
            {
                _indexerStore.Index(value);

                Interlocked.Increment(ref _count);
            }

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

    public async ValueTask<bool> ExistsAsync(int id)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        return await _documentRepository.ExistsAsync(id, true);
    }

    public async IAsyncEnumerable<Document> ListAsync(int take, int skip)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        await foreach (var (_, value) in _documentRepository.ReadRangeAsync(take, skip, true))
        {
            yield return value;
        }
    }

    public async ValueTask<(List<SearchRecordDto> searchRecords, int count)> SearchAsync(
        SearchQueryDto searchQueryDto, int take,
        int skip)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        var searchResults = _indexerStore.Search(searchQueryDto).ToList();

        searchResults.Sort((a, b) => b.SearchMetadata.Score.CompareTo(a.SearchMetadata.Score));

        var searchRecords = new List<SearchRecordDto>();

        foreach (var searchResult in searchResults.Skip(skip).Take(take))
        {
            var document = await _documentRepository.ReadAsync(searchResult.DocumentId, true);
            if (document != null)
            {
                searchRecords.Add(new SearchRecordDto
                {
                    DocumentDto = document.ToDto(),
                    MetadataList = searchResult.SearchMetadata.ToSearchMetadataList()
                });
            }
        }

        return (searchRecords, searchResults.Count);
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

            document.Id = currentId;

            await _identifierRepository.WriteAsync(0, currentId, true);

            await _documentRepository.WriteAsync(currentId, document, true);

            Interlocked.Increment(ref _count);

            _indexerStore.Index(document);
        });

        PendingActions.Enqueue(addAction);
    }

    public async ValueTask<bool> RemoveAsync(int id)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotWrite();

        var document = await _documentRepository.ReadAsync(id, true);

        if (document == null)
        {
            return false;
        }

        var removeAction = new Func<ValueTask>(async () =>
        {
            await _documentRepository.DeleteAsync(id, true);

            Interlocked.Decrement(ref _count);

            _indexerStore.Delete(document);
        });

        PendingActions.Enqueue(removeAction);

        return true;
    }
}
