using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Persistify.Storage;

public class InMemoryStorage : IStorage
{
    private readonly ConcurrentDictionary<string, string> _storage = new();
    public ValueTask SaveBlobAsync(string key, string data, CancellationToken cancellationToken = default)
    {
        _storage[key] = data;
        
        return ValueTask.CompletedTask;
    }

    public ValueTask<string> LoadBlobAsync(string key, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(_storage[key]);
    }

    public ValueTask DeleteBlobAsync(string key, CancellationToken cancellationToken = default)
    {
        _storage.TryRemove(key, out _);
        
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> ExistsBlobAsync(string key, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(_storage.ContainsKey(key));
    }
}