using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;

namespace Persistify.DataStructures.Tries;

public class ConcurrentByteMapTrie<TItem> : ITrie<TItem>
{
    [JsonProperty]
    private readonly TrieNode<TItem> _root;
    private readonly ReaderWriterLockSlim _lock;

    public ConcurrentByteMapTrie()
    {
        _root = new TrieNode<TItem>();
        _lock = new ReaderWriterLockSlim();
    }
    public void Add(string key, TItem item)
    {
        _lock.EnterWriteLock();
        try
        {
            _root.Add(key.AsSpan(), item);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public IEnumerable<TItem> Search(string key)
    {
        _lock.EnterReadLock();
        try
        {
            return _root.Search(key.AsSpan());
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public int Remove(Predicate<TItem> predicate)
    {
        _lock.EnterWriteLock();
        try
        {
            return _root.Remove(predicate);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }
}