using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Persistence.Core.Abstractions;

public interface ILongLinearRepository
{
    ValueTask WriteAsync(int id, long value);
    ValueTask<long?> ReadAsync(int id);
    ValueTask<IEnumerable<long>> ReadAllAsync();
    ValueTask RemoveAsync(int id);
}
