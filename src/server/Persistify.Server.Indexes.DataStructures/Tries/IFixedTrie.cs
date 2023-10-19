using System;
using System.Collections.Generic;

namespace Persistify.Server.Indexes.DataStructures.Tries;

public interface IFixedTrie<in TIndexItem, in TSearchItem, TItem>
    where TIndexItem : IndexFixedTrieItem<TItem>
    where TSearchItem : SearchFixedTrieItem
{
    void Insert(TIndexItem item);
    IEnumerable<TItem> Search(TSearchItem item);
    int UpdateIf(Predicate<TItem> predicate, Action<TItem> action);
}
