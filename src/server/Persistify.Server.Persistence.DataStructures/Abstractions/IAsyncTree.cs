using System.Threading.Tasks;

namespace Persistify.Server.Persistence.DataStructures.Abstractions;

public interface IAsyncTree<T>
{
    ValueTask InsertAsync(T value);
    ValueTask<T?> GetAsync(T value);
    ValueTask RemoveAsync(T value);
}
