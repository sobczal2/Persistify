using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Persistence.Core.Tests.Unit;

public class DictionaryLinearRepository : ILinearRepository
{
    private readonly Dictionary<long, long> _data;
    public DictionaryLinearRepository()
    {
        _data = new Dictionary<long, long>();
    }
    public ValueTask WriteAsync(long id, long value)
    {
        _data[id] = value;
        return ValueTask.CompletedTask;
    }

    public ValueTask<long?> ReadAsync(long id)
    {
        return ValueTask.FromResult(_data.TryGetValue(id, out var value) ? value : default(long?));
    }

    public ValueTask<IEnumerable<(long Id, long Value)>> ReadAllAsync()
    {
        return ValueTask.FromResult<IEnumerable<(long Id, long Value)>>(_data.Select(x => (x.Key, x.Value)));
    }

    public ValueTask RemoveAsync(long id)
    {
        _data.Remove(id);
        return ValueTask.CompletedTask;
    }

    public async ValueTask<long> CountAsync()
    {
        return await Task.FromResult(_data.Count);
    }
}
