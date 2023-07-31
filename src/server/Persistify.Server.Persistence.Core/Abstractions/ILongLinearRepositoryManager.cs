namespace Persistify.Server.Persistence.Core.Abstractions;

public interface ILongLinearRepositoryManager
{
    void Create(string repositoryName);
    ILongLinearRepository Get(string repositoryName);
    void Delete(string repositoryName);
}
