using Persistify.Storage;

namespace Persistify.Common.Lifecycle;

public interface IPersistedService
{
    Task LoadAsync(IStorageProvider storageProvider);
    Task SaveAsync(IStorageProvider storageProvider);
}