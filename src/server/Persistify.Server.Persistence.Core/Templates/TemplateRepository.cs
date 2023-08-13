using System.Threading.Tasks;
using Persistify.Domain.Templates;

namespace Persistify.Server.Persistence.Core.Templates;

public class TemplateRepository : ITemplateRepository
{

    public async ValueTask<Template?> GetAsync(int id)
    {
        throw new System.NotImplementedException();
    }

    public async ValueTask AddAsync(Template template)
    {
        throw new System.NotImplementedException();
    }

    public async ValueTask<bool> RemoveAsync(int id)
    {
        throw new System.NotImplementedException();
    }
}
