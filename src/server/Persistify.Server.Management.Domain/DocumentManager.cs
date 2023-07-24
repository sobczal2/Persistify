using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.Management.Domain.Abstractions;
using Persistify.Server.Management.Domain.Exceptions;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Management.Domain;

public class DocumentManager : IDocumentManager
{
    private readonly ITemplateManager _templateManager;
    private readonly IDocumentIdManager _documentIdManager;
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly ConcurrentDictionary<int, string> _templateIdRepositoryKeyMap;
    private readonly ConcurrentDictionary<int, SemaphoreSlim> _semaphores;

    public DocumentManager(
        ITemplateManager templateManager,
        IDocumentIdManager documentIdManager,
        IRepositoryFactory repositoryFactory
    )
    {
        _templateManager = templateManager;
        _documentIdManager = documentIdManager;
        _repositoryFactory = repositoryFactory;
        _templateIdRepositoryKeyMap = new ConcurrentDictionary<int, string>();
        _semaphores = new ConcurrentDictionary<int, SemaphoreSlim>();
    }

    public async ValueTask<long> IndexAsync(int templateId, Document document)
    {
        var @lock = _semaphores.GetOrAdd(templateId, _ => new SemaphoreSlim(1, 1));
        await @lock.WaitAsync();
        await _templateManager.LockTemplateAsync(templateId);
        try
        {
            await IndexInternalAsync(templateId, document);
        }
        finally
        {
            _templateManager.UnlockTemplate(templateId);
            @lock.Release();
        }

        return document.Id;
    }

    public async ValueTask<Document?> GetAsync(int templateId, long documentId)
    {
        var (repository, _) = GetRepositoryAndTemplate(templateId);
        return await repository.ReadAsync(documentId);
    }

    public async ValueTask DeleteAsync(int templateId, long documentId)
    {
        var @lock = _semaphores.GetOrAdd(templateId, _ => new SemaphoreSlim(1, 1));
        await @lock.WaitAsync();
        await _templateManager.LockTemplateAsync(templateId);
        try
        {
            await DeleteAsyncInternal(templateId, documentId);
        }
        finally
        {
            _templateManager.UnlockTemplate(templateId);
            @lock.Release();
        }
    }

    private async ValueTask<long> IndexInternalAsync(int templateId, Document document)
    {
        var (repository, template) = GetRepositoryAndTemplate(templateId);

        // TODO: Validate document against template

        var documentId = await _documentIdManager.GetNextId(templateId);
        document.Id = documentId;

        await repository.WriteAsync(documentId, document);

        // TODO: Add to type managers

        return documentId;
    }

    private async ValueTask DeleteAsyncInternal(int templateId, long documentId)
    {
        var (repository, _) = GetRepositoryAndTemplate(templateId);
        await repository.DeleteAsync(documentId);
    }

    private Template GetRequiredTemplate(int templateId)
    {
        var template = _templateManager.Get(templateId);
        if (template is null)
        {
            throw new TemplateNotFoundException(templateId);
        }

        return template;
    }

    private (IRepository<Document> Repository, Template Template) GetRepositoryAndTemplate(int templateId)
    {
        // TODO: management of document repositories should be a responsibility of template manager
        var template = GetRequiredTemplate(templateId);
        var repositoryKey = _templateIdRepositoryKeyMap.GetOrAdd(templateId, id => $"DocumentRepository_{id}");
        return (_repositoryFactory.Create<Document>(repositoryKey), template);
    }
}
