using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.LowLevel.Abstractions;

public interface ILowLevelStreamRepository<TValue>
{
    ValueTask<TValue> ReadAsync(int key, bool useLock = true);
    ValueTask<Dictionary<int, TValue>> ReadAllAsync(bool useLock = true);
    ValueTask WriteAsync(int key, TValue value, bool useLock = true);
    ValueTask DeleteAsync(int key, bool useLock = true);
    void Clear(bool useLock = true);
    bool IsValueEmpty(TValue value);
    TValue EmptyValue { get; }
}
