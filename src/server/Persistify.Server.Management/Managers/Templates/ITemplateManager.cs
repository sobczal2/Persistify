using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Templates;

namespace Persistify.Server.Management.Managers.Templates;

public interface ITemplateManager : IManager
{
    ValueTask<Template?> GetAsync(string templateName);
    ValueTask<Template?> GetAsync(int id);
    bool Exists(string templateName);
    ValueTask<List<Template>> ListAsync(int take, int skip);
    int Count();
    void Add(Template template);
    ValueTask<bool> RemoveAsync(int id);
}
