using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Abstractions;

public interface IValueTypeStreamRepository<TValue>
{
    ValueTask<TValue> ReadAsync(int key, bool useLock);
    ValueTask<Dictionary<int, TValue>> ReadAllAsync(bool useLock);
    ValueTask WriteAsync(int key, TValue value, bool useLock);
    ValueTask<bool> DeleteAsync(int key, bool useLock);
    void Clear(bool useLock);
    bool IsValueEmpty(TValue value);
    TValue EmptyValue { get; }
}
