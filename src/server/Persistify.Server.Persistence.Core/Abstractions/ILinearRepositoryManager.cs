namespace Persistify.Server.Persistence.Core.Abstractions;

public interface ILinearRepositoryManager
{
    void Create(string repositoryName);
    ILinearRepository Get(string repositoryName);
    void Delete(string repositoryName);
}
