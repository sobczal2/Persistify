using System;

namespace Persistify.Server.Indexes.DataStructures.Tries;

public abstract class IndexFixedTrieItem<TItem> : FixedTrieItem
{
    public abstract TItem Value { get; }
    public abstract void Merge(TItem other);
    public abstract bool IsEmpty { get; }
}
