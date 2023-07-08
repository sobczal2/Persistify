using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Persistance.Document;

public interface IDocumentStorage
{
    ValueTask AddAsync(string templateName, ulong documentId, Protos.Documents.Shared.Document document);
    ValueTask<Protos.Documents.Shared.Document?> GetAsync(string templateName, ulong documentId);
    ValueTask<IEnumerable<Protos.Documents.Shared.Document>> GetAllAsync(string templateName);
    ValueTask DeleteAsync(string templateName, ulong documentId);
    ValueTask InitializeForTemplateAsync(string templateName);
}
