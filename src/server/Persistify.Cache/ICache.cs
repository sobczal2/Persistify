namespace Persistify.Cache;

public interface ICache<in TKey, TValue> : ICache
    where TKey : notnull
{
    TValue? Get(TKey key);
    void Set(TKey key, TValue value);
    void Remove(TKey key);
}

public interface ICache
{
    void Clear();
}
