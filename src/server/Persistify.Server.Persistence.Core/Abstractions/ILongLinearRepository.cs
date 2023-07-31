using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Core.Abstractions;

public interface ILongLinearRepository
{
    ValueTask<long?> ReadAsync(int id, bool useLock = true);
    ValueTask<IDictionary<int, long>> ReadAllAsync(bool useLock = true);
    ValueTask WriteAsync(int id, long value, bool useLock = true);
    ValueTask DeleteAsync(int id, bool useLock = true);
    void ClearAsync(bool useLock = true);
}
