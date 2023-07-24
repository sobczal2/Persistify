using System.Threading.Tasks;

namespace Persistify.Server.Persistence.DataStructures.Providers;

public interface IStorageProvider<T>
    where T : notnull
{
    public ValueTask WriteAsync(long id, T value);
    public ValueTask<T?> ReadAsync(long id);
    public ValueTask RemoveAsync(long id);
}
