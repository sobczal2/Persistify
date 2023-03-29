using System;
using System.Collections.Generic;

namespace Persistify.DataStructures.Tries.Abstractions
{
    public interface ITrie<TItem> where TItem : struct, IComparable<TItem>, IConvertible
    {
        void Add(string key, TItem item);
        IEnumerable<TItem> Get(string key);
        IEnumerable<TItem> GetPrefix(string prefix);
        bool Contains(string key);
        bool ContainsPrefix(string prefix);
        bool Remove(string key);
        bool Remove(TItem item);
        void Clear();
    }
}