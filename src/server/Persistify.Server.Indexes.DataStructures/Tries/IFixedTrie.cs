using System;
using System.Collections.Generic;

namespace Persistify.Server.Indexes.DataStructures.Tries;

public interface IFixedTrie<in TIndexItem, in TSearchItem, out TItem>
    where TIndexItem : IndexFixedTrieItem<TItem>
    where TSearchItem : SearchFixedTrieItem
{
    int Depth { get; }
    void Insert(TIndexItem item);
    IEnumerable<TItem> Search(TSearchItem item);
    int UpdateIf(Predicate<TItem> predicate, Action<TItem> action);
}
