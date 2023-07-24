using System.Collections.Generic;

namespace Persistify.Server.Persistence.Cache;

public interface ICache<T>
{
    T? Get(long id);
    bool Exists(long id);
    void Set(long id, T value);
    void Remove(long id);
    IEnumerable<T> GetAll();
    void Clear();
}
