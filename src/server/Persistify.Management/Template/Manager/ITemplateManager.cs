using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Management.Template.Manager;

public interface ITemplateManager
{
    ValueTask AddAsync(Protos.Templates.Shared.Template template);
    ValueTask<Protos.Templates.Shared.Template?> GetAsync(string templateName);
    ValueTask<bool> ExistsAsync(string templateName);
    ValueTask<IEnumerable<Protos.Templates.Shared.Template>> GetAllAsync();
    ValueTask DeleteAsync(string templateName);
}
