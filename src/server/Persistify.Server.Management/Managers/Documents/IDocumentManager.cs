using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;

namespace Persistify.Server.Management.Managers.Documents;

public interface IDocumentManager : IManager
{
    ValueTask<Document?> GetAsync(int id);
    ValueTask<List<Document>> ListAsync(int take, int skip);
    int Count();
    void Add(Document document);
    ValueTask<bool> RemoveAsync(int id);
}
