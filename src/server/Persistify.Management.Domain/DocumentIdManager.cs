using System.Threading.Tasks;
using Persistify.Management.Domain.Abstractions;
using Persistify.Management.Domain.Exceptions;
using Persistify.Persistence.Core.Abstractions;

namespace Persistify.Management.Domain;

public class DocumentIdManager : IDocumentIdManager
{
    private const string DocumentIdKey = "DocumentId";
    private readonly ILongLinearRepository _linearRepository;

    public DocumentIdManager(
        ILinearRepositoryFactory linearRepositoryFactory
    )
    {
        _linearRepository = linearRepositoryFactory.CreateLong(DocumentIdKey);
    }

    public async ValueTask<long> GetNextId(int templateId)
    {
        var currentId = await _linearRepository.ReadAsync(templateId);
        var nextId = currentId is null ? 1 : currentId.Value + 1;
        await _linearRepository.WriteAsync(templateId, nextId);
        return nextId;
    }

    public async ValueTask<long> GetCurrentId(int templateId)
    {
        var currentId = await _linearRepository.ReadAsync(templateId);

        if (currentId is null)
        {
            throw new DocumentIdManagerInternalException("Current id is null");
        }

        return currentId.Value;
    }

    public async ValueTask RemoveId(int templateId)
    {
        await _linearRepository.RemoveAsync(templateId);
    }
}
