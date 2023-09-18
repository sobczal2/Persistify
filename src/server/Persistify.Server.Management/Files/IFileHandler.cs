namespace Persistify.Server.Management.Files;

public interface IFileHandler
{
    void EnsureRequiredFiles();
    void CreateFilesForTemplate(int templateId);
    void DeleteFilesForTemplate(int templateId);
}
