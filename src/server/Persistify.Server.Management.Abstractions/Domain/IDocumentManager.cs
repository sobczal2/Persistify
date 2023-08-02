using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;

namespace Persistify.Server.Management.Abstractions.Domain;

public interface IDocumentManager
{
    ValueTask AddAsync(int templateId, Document document);
    ValueTask<Document?> GetAsync(int templateId, int documentId);
    ValueTask DeleteAsync(int templateId, int documentId);
}
