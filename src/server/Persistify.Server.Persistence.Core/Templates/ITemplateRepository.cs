using System.Threading.Tasks;
using Persistify.Domain.Templates;

namespace Persistify.Server.Persistence.Core.Templates;

public interface ITemplateRepository
{
    ValueTask<Template?> GetAsync(int id);
    void Add(Template template);
    void Remove(int id);
}
