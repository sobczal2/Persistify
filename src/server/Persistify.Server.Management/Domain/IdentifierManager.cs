using System.Threading.Tasks;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Management.Domain;

public class IdentifierManager : IIdentifierManager
{
    private readonly IIntLinearRepository _intLinearRepository;
    private const string RepositoryName = "Identifier";

    public IdentifierManager(
        IIntLinearRepositoryManager linearRepositoryManager
    )
    {
        linearRepositoryManager.Create(RepositoryName);
        _intLinearRepository = linearRepositoryManager.Get(RepositoryName);
    }

    public async ValueTask<int> NextTemplateIdAsync()
    {
        var next = (await _intLinearRepository.ReadAsync(0, false) ?? -1) + 1;
        await _intLinearRepository.WriteAsync(0, next, false);
        return next;
    }

    public async ValueTask<int> NextIdForTemplateAsync(int templateId)
    {
        var repositoryIndex = templateId + 1;
        var next = (await _intLinearRepository.ReadAsync(repositoryIndex, false) ?? -1) + 1;
        await _intLinearRepository.WriteAsync(repositoryIndex, next, false);
        return next;
    }
}
