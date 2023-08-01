using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Core.Abstractions;

public interface IIntLinearRepository
{
    ValueTask<int?> ReadAsync(int id, bool useLock = true);
    ValueTask<IDictionary<int, int>> ReadAllAsync(bool useLock = true);
    ValueTask WriteAsync(int id, int value, bool useLock = true);
    ValueTask DeleteAsync(int id, bool useLock = true);
    void Clear(bool useLock = true);
}
