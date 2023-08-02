using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Management.Abstractions.Domain;

public interface ITemplateManager
{
    ValueTask AddAsync(Template template);
    ValueTask<Template?> GetAsync(int id);
    ValueTask DeleteAsync(int id);
    IRepository<Document> GetDocumentRepository(int templateId);
}
