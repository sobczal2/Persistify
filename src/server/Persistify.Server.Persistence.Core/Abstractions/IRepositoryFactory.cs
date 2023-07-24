namespace Persistify.Server.Persistence.Core.Abstractions;

public interface IRepositoryFactory
{
    IRepository<T> Create<T>(string repositoryName);
}
