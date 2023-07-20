using System;
using System.Collections.Generic;

namespace Persistify.DataStructures.PrefixTree;

public interface ITrie<TValue>
{
    void Add(string key, TValue value);
    void AddRange(IEnumerable<KeyValuePair<string, TValue>> items);
    ICollection<TValue> Search(string key, bool caseSensitive, bool exact);
    void Remove(Predicate<TValue> predicate);
}
