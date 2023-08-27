namespace Persistify.Server.Persistence.Core.Files;

public interface IFileProvider
{
    bool Exists(string relativePath);
    void Create(string relativePath);
    void Delete(string relativePath);
}
