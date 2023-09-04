using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Templates;

namespace Persistify.Server.Management.Managers.Templates;

public interface ITemplateManager : IManager
{
    ValueTask<Template?> GetAsync(int id);
    ValueTask<List<Template>> ListAsync(int take, int skip);
    ValueTask<int> CountAsync();
    void Add(Template template);
    ValueTask<bool> RemoveAsync(int id);
}
