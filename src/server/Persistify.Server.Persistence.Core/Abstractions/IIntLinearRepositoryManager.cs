namespace Persistify.Server.Persistence.Core.Abstractions;

public interface IIntLinearRepositoryManager
{
    void Create(string repositoryName);
    IIntLinearRepository Get(string repositoryName);
    void Delete(string repositoryName);
}
