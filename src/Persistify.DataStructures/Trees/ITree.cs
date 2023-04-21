using System;
using System.Collections.Generic;

namespace Persistify.DataStructures.Trees;

public interface ITree<TItem>
{
    void Insert(TItem item, double value);
    IEnumerable<TItem> Search(double min, double max);
    int Remove(Predicate<TItem> predicate);
}