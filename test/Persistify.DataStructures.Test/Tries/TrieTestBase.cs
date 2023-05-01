using Persistify.DataStructures.Tries;

namespace Persistify.DataStructures.Test.Tries;

public class TrieTestBase
{
    protected ConcurrentByteMapTrie<string> Trie;

    public TrieTestBase()
    {
        Trie = new ConcurrentByteMapTrie<string>();
    }
}