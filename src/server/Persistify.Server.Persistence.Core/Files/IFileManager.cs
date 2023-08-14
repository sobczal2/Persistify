using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Core.Files;

public interface IFileManager
{
    void EnsureRequiredFilesAsync();
    void CreateFilesForTemplateAsync(int templateId);
    void DeleteFilesForTemplateAsync(int templateId);
}
