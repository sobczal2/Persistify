using System.Threading.Tasks;

namespace Persistify.Management.Domain.Abstractions;

public interface IDocumentIdManager
{
    ValueTask<long> GetNextId(int templateId);
    ValueTask<long> GetCurrentId(int templateId);
    ValueTask RemoveId(int templateId);
}
