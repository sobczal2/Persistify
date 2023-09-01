using System.Threading.Tasks;
using Persistify.Domain.Templates;

namespace Persistify.Server.Management.Managers.Templates;

public interface ITemplateManager : IManager
{
    ValueTask<Template?> GetAsync(int id);
    void Add(Template template);
    void Remove(int id);
}
