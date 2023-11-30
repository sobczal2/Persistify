using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Server.Domain.Templates;

namespace Persistify.Server.Management.Managers.Templates;

public interface ITemplateManager : IManager
{
    ValueTask<Template?> GetAsync(
        string templateName
    );

    bool Exists(
        string templateName
    );

    IAsyncEnumerable<Template> ListAsync(
        int take,
        int skip
    );

    int Count();

    void Add(
        Template template
    );

    ValueTask<bool> RemoveAsync(
        int id
    );
}
