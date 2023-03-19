namespace Persistify.Core.Storage;

public interface IStorageProvider
{
    Task Initialize();
    Task<string[]> GetTypes();
    Task<(string value, int id)[]> GetIndexes(string type);
    Task<string?> GetRecord(string type, int id);
    Task<int> AddRecord(string type, string record);
    Task AddIndex(string type, string value, int recordId);
}