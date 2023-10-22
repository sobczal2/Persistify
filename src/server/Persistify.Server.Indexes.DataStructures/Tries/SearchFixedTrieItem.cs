namespace Persistify.Server.Indexes.DataStructures.Tries;

public abstract class SearchFixedTrieItem : FixedTrieItem
{
    public abstract int AnyIndex { get; }
    public abstract int RepeatedAnyIndex { get; }
}
