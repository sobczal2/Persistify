using System.Collections.Generic;

namespace Persistify.Server.Persistence.Core.Documents;

public interface IDocumentRepositoryManager
{
    IDocumentRepository GetRepository(int templateId);
    void AddRepository(int templateId);
    void DeleteRepository(int templateId);
}
