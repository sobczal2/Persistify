using System.Threading.Tasks;

namespace Persistify.Persistance.KeyValue;

public interface IKeyValueStorage
{
    ValueTask SetAsync<TValue>(string key, TValue value);
    ValueTask<TValue?> GetAsync<TValue>(string key);
    ValueTask DeleteAsync(string key);
}
