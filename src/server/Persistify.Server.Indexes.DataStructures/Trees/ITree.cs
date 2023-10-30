using System;
using System.Collections.Generic;

namespace Persistify.Server.Indexes.DataStructures.Trees;

public interface ITree<TItem>
    where TItem : IComparable<TItem>
{
    void Insert(TItem item);
    List<TItem> Search<TValue>(TValue min, TValue max, Func<TItem, TValue, int> comparer);
    int Remove(Predicate<TItem> predicate);
}
