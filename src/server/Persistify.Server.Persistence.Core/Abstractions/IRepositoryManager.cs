namespace Persistify.Server.Persistence.Core.Abstractions;

public interface IRepositoryManager
{
    void Create<T>(string repositoryName);
    IRepository<T> Get<T>(string repositoryName);
    bool Exists<T>(string repositoryName);
    void Delete<T>(string repositoryName);
}
