using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Core.Abstractions;

public interface IRepository<T>
{
    ValueTask WriteAsync(long id, T value);
    ValueTask<T?> ReadAsync(long id);
    ValueTask<IEnumerable<T>> ReadAllAsync();
    ValueTask<bool> ExistsAsync(long id);
    ValueTask<T?> DeleteAsync(long id);
}
