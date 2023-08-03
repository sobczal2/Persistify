using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Management.Domain;

public class TemplateManager : ITemplateManager
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IIdentifierManager _identifierManager;
    private readonly IRepository<Template> _templateRepository;

    private const string RepositoryName = "Template";

    public TemplateManager(
        IRepositoryManager repositoryManager,
        IIdentifierManager identifierManager
    )
    {
        _repositoryManager = repositoryManager;
        _identifierManager = identifierManager;
        _repositoryManager.Create<Template>(RepositoryName);
        _templateRepository = _repositoryManager.Get<Template>(RepositoryName);
    }

    public async ValueTask AddAsync(Template template)
    {
        var id = await _identifierManager.NextTemplateIdAsync();
        template.Id = id;
        await _templateRepository.WriteAsync(id, template);
        _repositoryManager.Create<Document>(GetRepositoryName(id));
    }

    public async ValueTask<Template?> GetAsync(int id)
    {
        return await _templateRepository.ReadAsync(id);
    }

    public async ValueTask<bool> DeleteAsync(int id)
    {
        if(!await _templateRepository.DeleteAsync(id))
        {
            return false;
        }
        _repositoryManager.Delete<Document>(GetRepositoryName(id));

        return true;
    }

    public IRepository<Document> GetDocumentRepository(int templateId)
    {
        return _repositoryManager.Get<Document>(GetRepositoryName(templateId));
    }

    private static string GetRepositoryName(int templateId) => $"Documents/{templateId:x8}";
}
