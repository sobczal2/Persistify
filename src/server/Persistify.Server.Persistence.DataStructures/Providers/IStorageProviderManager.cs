namespace Persistify.Server.Persistence.DataStructures.Providers;

public interface IStorageProviderManager
{
    void Create<T>(string name) where T : notnull;
    IStorageProvider<T> Get<T>(string name) where T : notnull;
    void Delete<T>(string name) where T : notnull;
}
