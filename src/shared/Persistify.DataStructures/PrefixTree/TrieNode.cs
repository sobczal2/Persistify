using System.Collections.Generic;

namespace Persistify.DataStructures.PrefixTree;

public class TrieNode<TValue>
{
    public TrieNode()
    {
        Children = new TrieNode<TValue>[62];
        Values = new HashSet<TValue>();
    }

    public TrieNode<TValue>?[] Children { get; set; }
    public HashSet<TValue> Values { get; set; }
}
