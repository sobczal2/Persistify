namespace Persistify.Server.Management.Files;

public interface IFileManager
{
    void EnsureRequiredFiles();
    void CreateFilesForTemplate(int templateId);
    void DeleteFilesForTemplate(int templateId);
}
