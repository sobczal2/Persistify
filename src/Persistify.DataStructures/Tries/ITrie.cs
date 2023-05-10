using System;
using System.Collections.Generic;

namespace Persistify.DataStructures.Tries;

public interface ITrie<TItem>
{
    void Add(string key, TItem item);
    IEnumerable<TItem> Search(string key, bool caseSensitive, bool exact);
    int Remove(Predicate<TItem> predicate);
}