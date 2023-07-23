namespace Persistify.Persistence.DataStructures.Providers;

public interface IStorageProviderFactory
{
    IStorageProvider<T> Create<T>(string name) where T : notnull;
}
