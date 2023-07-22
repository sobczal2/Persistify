namespace Persistify.Persistence.Cache;

public class LruCache<T> : ICache<T>
{
    private readonly int _capacity;
    private readonly Dictionary<long, LinkedListNode<(long id, T value)>> _dict;
    private readonly LinkedList<(long id, T value)> _list;

    public LruCache(int capacity)
    {
        _capacity = capacity;
        _list = new LinkedList<(long id, T value)>();
        _dict = new Dictionary<long, LinkedListNode<(long id, T value)>>();
    }

    public T? Get(long id)
    {
        if (!_dict.TryGetValue(id, out var node))
        {
            return default;
        }

        _list.Remove(node);
        _list.AddFirst(node);
        return node.Value.value;
    }

    public bool Exists(long id)
    {
        return _dict.ContainsKey(id);
    }

    public void Set(long id, T value)
    {
        if (Exists(id))
        {
            _list.Remove(_dict[id]);
        }
        else if (_dict.Count >= _capacity)
        {
            _dict.Remove(_list.Last!.Value.id);
            _list.RemoveLast();
        }

        var node = new LinkedListNode<(long id, T value)>((id, value));
        _list.AddFirst(node);
        _dict[id] = node;
    }

    public void Remove(long id)
    {
        if (!_dict.TryGetValue(id, out var node))
        {
            return;
        }

        _list.Remove(node);
        _dict.Remove(id);
    }

    public IEnumerable<T> GetAll()
    {
        return _list.Select(x => x.value);
    }

    public void Clear()
    {
        _list.Clear();
        _dict.Clear();
    }
}
