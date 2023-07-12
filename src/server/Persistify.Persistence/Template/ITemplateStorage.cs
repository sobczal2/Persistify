using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Persistance.Template;

public interface ITemplateStorage
{
    ValueTask AddAsync(Protos.Templates.Shared.Template template);
    ValueTask<IEnumerable<Protos.Templates.Shared.Template>> GetAllAsync();
    ValueTask DeleteAsync(string templateName);
    ValueTask<ConcurrentDictionary<string, long>> GetTemplateIdsAsync();
    ValueTask SaveTemplateIdsAsync(ConcurrentDictionary<string, long> templateIds);
}
