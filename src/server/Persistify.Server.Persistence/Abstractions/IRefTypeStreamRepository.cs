using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Abstractions;

public interface IRefTypeStreamRepository<TValue>
{
    ValueTask<TValue?> ReadAsync(int key, bool useLock);
    ValueTask<bool> ExistsAsync(int key, bool useLock);
    ValueTask<Dictionary<int, TValue>> ReadAllAsync(bool useLock);
    ValueTask WriteAsync(int key, TValue value, bool useLock);
    ValueTask<bool> DeleteAsync(int key, bool useLock);
    void Clear(bool useLock);
}
