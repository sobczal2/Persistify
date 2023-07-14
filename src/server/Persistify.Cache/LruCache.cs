using System.Collections.Generic;

namespace Persistify.Cache;

public class LruCache<TKey, TValue> : ICache<TKey, TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, LinkedListNode<(TKey key, TValue value)>> _cache;
    private readonly int _capacity;
    private readonly LinkedList<(TKey key, TValue value)> _lruList;

    public LruCache(int capacity)
    {
        _capacity = capacity;
        _cache = new Dictionary<TKey, LinkedListNode<(TKey key, TValue value)>>(capacity);
        _lruList = new LinkedList<(TKey key, TValue value)>();
    }

    public TValue? Get(TKey key)
    {
        if (!_cache.TryGetValue(key, out var node))
        {
            return default;
        }

        _lruList.Remove(node);
        _lruList.AddFirst(node);
        return node.Value.value;
    }

    public void Set(TKey key, TValue value)
    {
        if (_cache.Count >= _capacity)
        {
            var lastNode = _lruList.Last;
            _cache.Remove(lastNode!.Value.key);
            _lruList.RemoveLast();
        }

        var newNode = new LinkedListNode<(TKey key, TValue value)>((key, value));
        _lruList.AddFirst(newNode);
        _cache.Add(key, newNode);
    }

    public void Remove(TKey key)
    {
        if (!_cache.TryGetValue(key, out var node))
        {
            return;
        }

        _lruList.Remove(node);
        _cache.Remove(key);
    }

    public void Clear()
    {
        _lruList.Clear();
        _cache.Clear();
    }
}
