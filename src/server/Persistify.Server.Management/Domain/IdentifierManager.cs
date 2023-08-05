using System.Threading.Tasks;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Management.Domain;

public class IdentifierManager : IIdentifierManager
{
    private readonly IIntLinearRepositoryManager _linearRepositoryManager;
    private const string RepositoryName = "Identifier";

    public IdentifierManager(
        IIntLinearRepositoryManager linearRepositoryManager
    )
    {
        _linearRepositoryManager = linearRepositoryManager;
        linearRepositoryManager.Create(RepositoryName);
    }

    public async ValueTask<int> NextTemplateIdAsync()
    {
        var repository = _linearRepositoryManager.Get(RepositoryName);
        var next = (await repository.ReadAsync(0, false) ?? -1) + 1;
        await repository.WriteAsync(0, next, false);
        return next;
    }

    public async ValueTask<int> NextIdForTemplateAsync(int templateId)
    {
        var repository = _linearRepositoryManager.Get(RepositoryName);
        var repositoryIndex = templateId + 1;
        var next = (await repository.ReadAsync(repositoryIndex, false) ?? -1) + 1;
        await repository.WriteAsync(repositoryIndex, next, false);
        return next;
    }
}
