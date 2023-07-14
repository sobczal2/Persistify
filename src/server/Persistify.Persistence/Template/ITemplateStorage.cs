using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Persistance.Template;

public interface ITemplateStorage
{
    ValueTask AddAsync(Protos.Templates.Shared.Template template);
    ValueTask<Protos.Templates.Shared.Template?> GetAsync(string templateName);
    ValueTask<bool> ExistsAsync(string templateName);
    ValueTask<IEnumerable<Protos.Templates.Shared.Template>> GetAllAsync();
    ValueTask DeleteAsync(string templateName);
}
