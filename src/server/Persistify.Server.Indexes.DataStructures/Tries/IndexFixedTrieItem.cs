namespace Persistify.Server.Indexes.DataStructures.Tries;

public abstract class IndexFixedTrieItem<TItem> : FixedTrieItem
{
    public abstract TItem Value { get; }
    public abstract bool IsEmpty { get; }
    public abstract void Merge(TItem other);
}
