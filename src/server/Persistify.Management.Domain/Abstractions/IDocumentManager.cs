using System.Threading.Tasks;
using Persistify.Domain.Documents;

namespace Persistify.Management.Domain.Abstractions;

public interface IDocumentManager
{
    ValueTask<long> IndexAsync(int templateId, Document document);
    ValueTask<Document?> GetAsync(int templateId, long documentId);
    ValueTask DeleteAsync(int templateId, long documentId);
}
