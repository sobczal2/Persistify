using Persistify.Requests.Documents;
using Persistify.Requests.Templates;

namespace Persistify.Tools.SeedDatabase;

public interface IDocumentsGenerator
{
    CreateTemplateRequest GetCreateTemplateRequest();
    CreateDocumentRequest GetCreateDocumentRequest();
}
