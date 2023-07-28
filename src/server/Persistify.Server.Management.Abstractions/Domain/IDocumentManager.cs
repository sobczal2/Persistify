using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;

namespace Persistify.Server.Management.Abstractions.Domain;

public interface IDocumentManager
{
    ValueTask<long> IndexAsync(int templateId, Document document);
    ValueTask<Document?> GetAsync(int templateId, long documentId);
    ValueTask DeleteAsync(int templateId, long documentId);
    ValueTask<IEnumerable<long>> AllIdsAsync(int templateId);
    ValueTask<List<Document>> GetDocumentsAsync(int templateId, List<long> documentIds);
}
