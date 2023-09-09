using System.Collections.Generic;

namespace Persistify.Server.Management.Managers.Documents;

public interface IDocumentManagerStore
{
    IDocumentManager? GetManager(int templateId);
    void AddManager(int templateId);
    void DeleteManager(int templateId);
    IEnumerable<IDocumentManager> GetManagers();
}
