namespace Persistify.Server.Management.Files;

public interface IFileManager
{
    void EnsureRequiredFilesAsync();
    void CreateFilesForTemplateAsync(int templateId);
    void DeleteFilesForTemplateAsync(int templateId);
}
