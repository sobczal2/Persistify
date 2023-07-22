namespace Persistify.Persistence.Cache;

public class RndCache<T> : ICache<T>
{
    private readonly int _capacity;
    private readonly Dictionary<long, T> _dict;

    public RndCache(int capacity)
    {
        _capacity = capacity;
        _dict = new Dictionary<long, T>();
    }

    public T? Get(long id)
    {
        return _dict.TryGetValue(id, out var value) ? value : default;
    }

    public bool Exists(long id)
    {
        return _dict.ContainsKey(id);
    }

    public void Set(long id, T value)
    {
        if (_dict.Count >= _capacity)
        {
            _dict.Remove(_dict.Keys.ElementAt(Random.Shared.Next(_dict.Count)));
        }

        _dict[id] = value;
    }

    public void Remove(long id)
    {
        _dict.Remove(id);
    }

    public IEnumerable<T> GetAll()
    {
        return _dict.Values;
    }

    public void Clear()
    {
        _dict.Clear();
    }
}
