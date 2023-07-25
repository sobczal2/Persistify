using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Core.Abstractions;

public interface ILinearRepository
{
    ValueTask WriteAsync(long id, long value);
    ValueTask<long?> ReadAsync(long id);
    ValueTask<IEnumerable<(long Id, long Value)>> ReadAllAsync();
    ValueTask RemoveAsync(long id);
    ValueTask<long> CountAsync();
}
