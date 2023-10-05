using Persistify.Domain.Templates;

namespace Persistify.Server.Files;

public interface IFileHandler
{
    void EnsureRequiredFiles();
    void CreateFilesForTemplate(Template template);
    void DeleteFilesForTemplate(Template template);
}
