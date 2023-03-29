using System.Linq;
using Persistify.DataStructures.Tries.Abstractions;
using Xunit;

namespace Persistify.DataStructures.Test.Tries;

public abstract class TrieTestBase<TTrie> where TTrie : ITrie<int>
{
    protected abstract TTrie CreateTrie();

    [Fact]
    public void Should_Add_10_Items_And_Get_Them()
    {
        var trie = CreateTrie();
        for (var i = 0; i < 10; i++) trie.Add(i.ToString("D5"), i);

        for (var i = 0; i < 10; i++)
        {
            Assert.True(trie.Contains(i.ToString("D5")));
            Assert.Single(trie.Get(i.ToString("D5")));
            Assert.Equal(i, trie.Get(i.ToString("D5")).First());
        }
    }

    [Fact]
    public void Should_Add_10_Items_And_Get_Their_Prefixes()
    {
        var trie = CreateTrie();
        for (var i = 0; i < 10; i++) trie.Add(i.ToString("D5"), i);

        for (var i = 0; i < 10; i++)
        {
            Assert.True(trie.ContainsPrefix(i.ToString("D5")));
            Assert.Single(trie.GetPrefix(i.ToString("D5")));
            Assert.Equal(i, trie.GetPrefix(i.ToString("D5")).First());
        }
    }
}