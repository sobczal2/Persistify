using System.Collections.Generic;

namespace Persistify.DataStructures.MultiTargetTries;

public interface IMultiTargetTrie<TItem>
    where TItem : notnull
{
    void Add(string key, TItem item, ISingleTargetMapper mapper);
    IEnumerable<TItem> Search(string query, IMultiTargetMapper mapper);
    bool Contains(string key, IMultiTargetMapper mapper);
    bool Remove(TItem item);
    MultiTargetTrieStats GetStats();
}