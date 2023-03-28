using System.Collections.Generic;

namespace Persistify.DataStructures.Tries.Abstractions;

public interface ITrie<TItem>
{
    public void Add(string key, TItem item);
    public IEnumerable<TItem> Get(string key);
    public IEnumerable<TItem> GetPrefix(string prefix);
    public bool Contains(string key);
    public bool ContainsPrefix(string prefix);
    public bool Remove(string key);
    public void Clear();
}