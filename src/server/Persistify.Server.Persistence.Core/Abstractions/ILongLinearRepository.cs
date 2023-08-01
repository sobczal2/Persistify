using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Core.Abstractions;

public interface ILongLinearRepository
{
    ValueTask<long?> ReadAsync(int key, bool useLock = true);
    ValueTask<IDictionary<int, long>> ReadAllAsync(bool useLock = true);
    ValueTask WriteAsync(int key, long value, bool useLock = true);
    ValueTask DeleteAsync(int key, bool useLock = true);
    void Clear(bool useLock = true);
}
