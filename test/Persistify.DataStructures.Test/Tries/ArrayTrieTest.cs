using Persistify.DataStructures.Tries;

namespace Persistify.DataStructures.Test.Tries;

public class ArrayTrieTest : TrieTestBase<ArrayTrie<int>>
{
    protected override ArrayTrie<int> CreateTrie()
    {
        return new ArrayTrie<int>();
    }
}