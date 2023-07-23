using System.Threading.Tasks;
using Persistify.Persistence.DataStructures.Providers;

namespace Persistify.Persistence.DataStructures.Tests.Unit;

public class ArrayStorageProvider<T> : IStorageProvider<T> where T : notnull
{
    private readonly T?[] _items;

    public ArrayStorageProvider(int capacity)
    {
        _items = new T?[capacity];
    }

    public ValueTask WriteAsync(long id, T value)
    {
        _items[id] = value;
        return ValueTask.CompletedTask;
    }

    public ValueTask<T?> ReadAsync(long id)
    {
        return new ValueTask<T?>(_items[id]);
    }

    public ValueTask RemoveAsync(long id)
    {
        _items[id] = default;
        return ValueTask.CompletedTask;
    }
}
