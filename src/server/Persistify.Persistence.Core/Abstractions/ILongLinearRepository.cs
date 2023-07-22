using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Persistence.Core.Abstractions;

public interface ILongLinearRepository
{
    ValueTask WriteAsync(long id, long value);
    ValueTask<long?> ReadAsync(long id);
    ValueTask<IEnumerable<(long Id, long Value)>> ReadAllAsync();
    ValueTask RemoveAsync(long id);
    ValueTask<long> CountAsync();
}
