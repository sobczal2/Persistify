using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Server.Management.Domain.Abstractions;
using Persistify.Server.Management.Domain.Exceptions;
using Persistify.Server.Management.Types.Abstractions;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Management.Domain.Implementations;

public class DocumentManager : IDocumentManager
{
    private readonly ITemplateManager _templateManager;
    private readonly IDocumentIdManager _documentIdManager;
    private readonly IEnumerable<ITypeManager> _typeManagers;

    public DocumentManager(
        ITemplateManager templateManager,
        IDocumentIdManager documentIdManager,
        IEnumerable<ITypeManager> typeManagers
    )
    {
        _templateManager = templateManager;
        _documentIdManager = documentIdManager;
        _typeManagers = typeManagers;
    }

    public async ValueTask<long> IndexAsync(int templateId, Document document)
    {
        return await _templateManager.PerformActionOnLockedTemplateAsync(templateId, async (template, repository) =>
        {
            // TODO: Validate against template

            document.Id = await _documentIdManager.GetNextId(templateId);
            await repository.WriteAsync(document.Id, document);

            foreach (var typeManager in _typeManagers)
            {
                await typeManager.IndexAsync(templateId, document);
            }

            return document.Id;
        });
    }

    public async ValueTask<Document?> GetAsync(int templateId, long documentId)
    {
        return await _templateManager.PerformActionOnLockedTemplateAsync(templateId,
            async (_, repository) => await repository.ReadAsync(documentId));
    }

    public async ValueTask DeleteAsync(int templateId, long documentId)
    {
        await _templateManager.PerformActionOnLockedTemplateAsync(templateId,
            async (_, repository) =>
            {
                await repository.DeleteAsync(documentId);

                foreach (var typeManager in _typeManagers)
                {
                    await typeManager.DeleteAsync(templateId, documentId);
                }
            });
    }
}
