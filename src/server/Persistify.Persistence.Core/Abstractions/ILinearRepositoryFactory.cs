namespace Persistify.Persistence.Core.Abstractions;

public interface ILinearRepositoryFactory
{
    ILongLinearRepository CreateLong(string repositoryName);
}
