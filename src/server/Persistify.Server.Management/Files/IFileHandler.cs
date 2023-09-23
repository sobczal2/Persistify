using Persistify.Domain.Templates;

namespace Persistify.Server.Management.Files;

public interface IFileHandler
{
    void EnsureRequiredFiles();
    void CreateFilesForTemplate(Template template);
    void DeleteFilesForTemplate(Template template);
}
