using System;
using System.Collections.Generic;

namespace Persistify.DataStructures.IntervalTree;

public interface IIntervalTree<TValue>
    where TValue : IComparable<TValue>, IComparable<double>
{
    void Add(TValue item);
    ICollection<TValue> Search(double min, double max);
    void Remove(Predicate<TValue> predicate);
}
