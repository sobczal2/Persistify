using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Core.Abstractions;

public interface IRepository<T>
    where T : class
{
    ValueTask<T?> ReadAsync(int id, bool useLock = true);
    ValueTask<IDictionary<int, T>> ReadAllAsync(bool useLock = true);
    ValueTask WriteAsync(int id, T value, bool useLock = true);
    ValueTask<bool> DeleteAsync(int id, bool useLock = true);
    void Clear(bool useLock = true);
}
