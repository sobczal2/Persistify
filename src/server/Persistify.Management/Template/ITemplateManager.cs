using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Management.Template;

public interface ITemplateManager
{
    ValueTask AddAsync(Protos.Templates.Shared.Template template);
    Protos.Templates.Shared.Template? Get(string templateName);
    bool Exists(string templateName);
    IEnumerable<Protos.Templates.Shared.Template> GetAll();
    ValueTask DeleteAsync(string templateName);
    ValueTask LoadAsync();
}
