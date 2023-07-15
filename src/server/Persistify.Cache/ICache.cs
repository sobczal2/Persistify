using System;

namespace Persistify.Cache;

public interface ICache<TKey, TValue> : ICache
    where TKey : notnull
{
    TValue? Get(TKey key);
    void Set(TKey key, TValue value);
    void Remove(TKey key);
    void Remove(Predicate<TKey> predicate);
}

public interface ICache
{
    void Clear();
}
