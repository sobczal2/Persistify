namespace Persistify.Server.Persistence.Core.Abstractions;

public interface IRepositoryManager
{
    void Create<T>(string repositoryName) where T : class;
    IRepository<T> Get<T>(string repositoryName) where T : class;
    bool Exists<T>(string repositoryName) where T : class;
    void Delete<T>(string repositoryName) where T : class;
}
