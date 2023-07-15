using System.Threading.Tasks;
using Persistify.HostedServices;

namespace Persistify.Management.Document.Manager;

public interface IDocumentManager : IActOnStartup, IActOnShutdown, IActRecurrently
{
    ValueTask<long> AddAsync(string templateName, Protos.Documents.Shared.Document document);
    ValueTask<Protos.Documents.Shared.Document?> GetAsync(string templateName, long documentId);
    ValueTask DeleteAsync(string templateName, long documentId);
    ValueTask<long> DeleteTemplateAsync(string templateName);
    ValueTask AddTemplateAsync(string templateName, long currentDocumentId);
}
