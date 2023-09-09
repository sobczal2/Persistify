using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Abstractions;

public interface IRefTypeStreamRepository<TValue>
{
    ValueTask<TValue?> ReadAsync(int key, bool useLock);
    ValueTask<bool> ExistsAsync(int key, bool useLock);
    ValueTask<List<(int Key, TValue Value)>> ReadRangeAsync(int take, int skip, bool useLock);
    ValueTask<int> CountAsync(bool useLock);
    ValueTask WriteAsync(int key, TValue value, bool useLock);
    ValueTask<bool> DeleteAsync(int key, bool useLock);
    void Clear(bool useLock);
}
