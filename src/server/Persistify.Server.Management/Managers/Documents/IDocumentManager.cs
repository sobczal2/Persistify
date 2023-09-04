using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;

namespace Persistify.Server.Management.Managers.Documents;

public interface IDocumentManager : IManager
{
    ValueTask<Document?> GetAsync(int id);
    ValueTask<List<Document>> ListAsync(int take, int skip);
    ValueTask<int> CountAsync();
    void Add(Document document);
    ValueTask<bool> RemoveAsync(int id);
}
