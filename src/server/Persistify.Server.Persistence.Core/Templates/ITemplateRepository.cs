using System.Threading.Tasks;
using Persistify.Domain.Templates;

namespace Persistify.Server.Persistence.Core.Templates;

public interface ITemplateRepository
{
    ValueTask<Template?> GetAsync(int id);
    ValueTask AddAsync(Template template);
    ValueTask<bool> RemoveAsync(int id);
}
