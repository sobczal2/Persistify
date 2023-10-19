namespace Persistify.Server.Indexes.DataStructures.Tries;

public abstract class FixedTrieItem
{
    public abstract int GetIndex(int index);
    public abstract int Length { get; }
}
