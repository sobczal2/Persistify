using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Persistence.Core.Abstractions;

public interface IRepository<T>
{
    ValueTask WriteAsync(long id, T value);
    ValueTask<T?> ReadAsync(long id);
    IAsyncEnumerable<T> ReadAllAsync();
    ValueTask<long> CountAsync();
    ValueTask<bool> ExistsAsync(long id);
    ValueTask DeleteAsync(long id);
}
