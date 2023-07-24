using System;
using System.Threading.Tasks;
using Persistify.Server.Persistence.DataStructures.Providers;

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
        if (id >= _items.Length)
        {
            throw new IndexOutOfRangeException();
        }

        _items[id] = value;
        return ValueTask.CompletedTask;
    }

    public ValueTask<T?> ReadAsync(long id)
    {
        if (id >= _items.Length)
        {
            throw new IndexOutOfRangeException();
        }

        return new ValueTask<T?>(_items[id]);
    }

    public ValueTask RemoveAsync(long id)
    {
        if (id >= _items.Length)
        {
            throw new IndexOutOfRangeException();
        }

        _items[id] = default;
        return ValueTask.CompletedTask;
    }
}
