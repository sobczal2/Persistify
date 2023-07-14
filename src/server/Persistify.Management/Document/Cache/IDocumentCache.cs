using Persistify.Cache;

namespace Persistify.Management.Document.Cache;

public interface IDocumentCache : ICache<(string templateName, long id), Protos.Documents.Shared.Document>
{
}
