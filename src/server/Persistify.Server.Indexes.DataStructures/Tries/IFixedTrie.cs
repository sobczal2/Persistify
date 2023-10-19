using System;
using System.Collections.Generic;

namespace Persistify.Server.Indexes.DataStructures.Tries;

public interface IFixedTrie<TItem>
{
    void Insert(IndexFixedTrieItem<TItem> item);
    IEnumerable<(TItem Item, int Distance)> Search(SearchFixedTrieItem item);
    int Remove(Predicate<TItem> predicate);
}
