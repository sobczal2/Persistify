using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Domain.Documents;
using Persistify.Domain.Search.Queries;
using Persistify.Domain.Templates;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Indexes.Indexers;
using Persistify.Server.Indexes.Searches;
using Persistify.Server.Management.Files;
using Persistify.Server.Management.Transactions;
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
        IOptions<RepositorySettings> repositorySettingsOptions
    ) : base(
        transactionState
    )
    {
        _template = template;

        _indexerStore = new IndexerStore(template);

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

        foreach (var (_, value) in kvList)
        {
            list.Add(value);
        }

        return list;
    }

    public async ValueTask<(List<Document> documents, int count)> SearchAsync(SearchQuery searchQuery, int take,
        int skip)
    {
        ThrowIfNotInitialized();
        ThrowIfCannotRead();

        var searchResults = await SearchInternalAsync(searchQuery);

        var documents = new List<Document>(searchResults.Count);

        searchResults.Sort((a, b) => b.Score.CompareTo(a.Score));

        foreach (var searchResult in searchResults.Skip(skip).Take(take))
        {
            var document = await _documentRepository.ReadAsync(searchResult.DocumentId, true);
            if (document != null)
            {
                documents.Add(document);
            }
        }

        return (documents, searchResults.Count);
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

            Interlocked.Increment(ref _count);

            await _indexerStore.IndexAsync(document);
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

            await _indexerStore.DeleteAsync(document);
        });

        PendingActions.Enqueue(removeAction);

        return true;
    }

    private ValueTask<List<ISearchResult>> SearchInternalAsync(SearchQuery searchQuery)
    {
        return ValueTask.FromResult(new List<ISearchResult>());
        // if (searchNode is BoolSearchNode boolSearchNode)
        // {
        //     return await _indexerStore.SearchAsync(new BoolSearchQuery(boolSearchNode.FieldName,
        //         boolSearchNode.Value, boolSearchNode.Boost));
        // }
        // else if (searchNode is NumberSearchNode numberSearchNode)
        // {
        //     return new List<ISearchResult>();
        // }
        // else if (searchNode is TextSearchNode textSearchNode)
        // {
        //     return new List<ISearchResult>();
        // }
        // else if (searchNode is AndSearchNode andSearchNode)
        // {
        //     var results = new IEnumerable<ISearchResult>[andSearchNode.Nodes.Count];
        //     for (var i = 0; i < results.Length; i++)
        //     {
        //         results[i] = await SearchInternalAsync(andSearchNode.Nodes[i]);
        //     }
        //
        //     return EnumerableHelpers
        //         .IntersectSorted(Comparer<ISearchResult>.Create((a, b) => a.DocumentId.CompareTo(b.DocumentId)),
        //             results).ToList();
        // }
        // else if (searchNode is OrSearchNode orSearchNode)
        // {
        //     var results = new IEnumerable<ISearchResult>[orSearchNode.Nodes.Count];
        //     for (var i = 0; i < results.Length; i++)
        //     {
        //         results[i] = await SearchInternalAsync(orSearchNode.Nodes[i]);
        //     }
        //
        //     return EnumerableHelpers
        //         .MergeSorted(Comparer<ISearchResult>.Create((a, b) => a.DocumentId.CompareTo(b.DocumentId)),
        //             results).ToList();
        // }
        // else
        // {
        //     throw new PersistifyInternalException();
        // }
    }
}
