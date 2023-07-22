using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Templates;

namespace Persistify.Management.Domain.Abstractions;

public interface ITemplateManager
{
    ValueTask CreateAsync(Template template);
    Template? Get(int id);
    IEnumerable<Template> GetAll();
    ValueTask DeleteAsync(int id);
}
