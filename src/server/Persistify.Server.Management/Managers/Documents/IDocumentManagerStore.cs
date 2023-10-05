using System.Collections.Generic;
using Persistify.Domain.Templates;

namespace Persistify.Server.Management.Managers.Documents;

public interface IDocumentManagerStore
{
    IDocumentManager? GetManager(int templateId);
    void AddManager(Template template);
    void DeleteManager(int templateId);
    IEnumerable<IDocumentManager> GetManagers();
}
