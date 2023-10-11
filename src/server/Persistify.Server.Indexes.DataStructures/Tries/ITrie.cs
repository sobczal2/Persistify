using System;
using System.Collections.Generic;

namespace Persistify.Server.Indexes.DataStructures.Tries;

public interface ITrie<TItem>
    where TItem : IComparable<TItem>, IEnumerable<int>
{
    void Insert(TItem item);
    int Remove(Predicate<TItem> predicate);
}
