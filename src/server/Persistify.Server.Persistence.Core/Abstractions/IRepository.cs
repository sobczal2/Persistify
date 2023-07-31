using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Core.Abstractions;

public interface IRepository<T>
    where T : class
{
    ValueTask<T?> ReadAsync(int id);
    ValueTask<IDictionary<int, T>> ReadAllAsync();
    ValueTask WriteAsync(int id, T value);
    ValueTask<bool> DeleteAsync(int id);
    void ClearAsync();
}
