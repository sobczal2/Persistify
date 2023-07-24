namespace Persistify.Server.Persistence.Core.Abstractions;

public interface ILinearRepositoryFactory
{
    ILongLinearRepository CreateLong(string repositoryName);
}
