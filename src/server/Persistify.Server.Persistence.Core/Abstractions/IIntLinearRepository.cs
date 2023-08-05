using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Core.Abstractions;

public interface IIntLinearRepository
{
    ValueTask<int?> ReadAsync(int key, bool useLock = true);
    ValueTask<IDictionary<int, int>> ReadAllAsync(bool useLock = true);
    ValueTask WriteAsync(int key, int value, bool useLock = true);
    ValueTask DeleteAsync(int key, bool useLock = true);
    void Clear(bool useLock = true);
}
