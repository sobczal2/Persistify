using System;

namespace Persistify.Server.Indexes.DataStructures.Tries;

public abstract class IndexFixedTrieItem<TSelf> : FixedTrieItem, IComparable<TSelf>, IComparable<IndexFixedTrieItem<TSelf>>
{
    public abstract int CompareTo(TSelf? other);
    public abstract TSelf Value { get; }

    public int CompareTo(IndexFixedTrieItem<TSelf>? other)
    {
        if (other == null)
        {
            return 1;
        }

        return CompareTo(other.Value);
    }
}
