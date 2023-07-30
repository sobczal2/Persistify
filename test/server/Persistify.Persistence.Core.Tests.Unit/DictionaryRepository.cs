using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Persistify.Server.Persistence.Core.Abstractions;
using ProtoBuf;

namespace Persistify.Persistence.Core.Tests.Unit;

public class DictionaryRepository<T> : IRepository<T>
{
    private readonly Dictionary<long, T> _data;
    public DictionaryRepository()
    {
        _data = new Dictionary<long, T>();
    }
    public ValueTask WriteAsync(long id, T value)
    {
        _data[id] = DeepCopy(value);
        return ValueTask.CompletedTask;
    }

    public ValueTask<T?> ReadAsync(long id)
    {
        return ValueTask.FromResult(_data.TryGetValue(id, out var value) ? DeepCopy(value) : default(T?));
    }

    public IAsyncEnumerable<T> ReadAllAsync()
    {
        return _data.Values.Select(DeepCopy).ToAsyncEnumerable();
    }

    public ValueTask<bool> ExistsAsync(long id)
    {
        return ValueTask.FromResult(_data.ContainsKey(id));
    }

    public ValueTask<T?> DeleteAsync(long id)
    {
        if (_data.TryGetValue(id, out var value))
        {
            _data.Remove(id);
            return ValueTask.FromResult<T?>(DeepCopy(value));
        }

        return ValueTask.FromResult(default(T?));
    }

    private T DeepCopy(T value)
    {
        using var stream = new MemoryStream();
        Serializer.Serialize(stream, value);
        stream.Position = 0;
        return Serializer.Deserialize<T>(stream);
    }
}
