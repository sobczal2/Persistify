using System;
using System.Collections.Generic;

namespace Persistify.DataStructures.Tries;

public interface ITrie<TItem>
{
    void Add(string key, TItem item);
    void AddRange(IEnumerable<KeyValuePair<string, TItem>> items);
    IEnumerable<TItem> Get(string key);
    IEnumerable<TItem> Search(string query);
    bool Contains(string key);
    bool Remove(TItem item);
    TrieStats GetStats();
    long UniqueItemsCount { get; }
}