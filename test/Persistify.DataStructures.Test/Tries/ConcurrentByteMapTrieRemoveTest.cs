using Xunit;

namespace Persistify.DataStructures.Test.Tries;

public class ConcurrentByteMapTrieRemoveTest : TrieTestBase
{
    [Fact]
    public void Remove_WhenTreeIsEmpty_ShouldDoNothing()
    {
        Trie.Remove(item => item == "Item1");

        var root = Trie.GetRoot();
        
        Assert.NotNull(root);
        Assert.Empty(root.GetItems()!);
    }

    [Fact]
    public void Remove_WhenItemExists_ShouldRemoveItem()
    {
        Trie.Add("b", "Item2");
        Trie.Remove(item => item == "Item2");
        Assert.Empty(Trie.Search("b"));
    }

    [Fact]
    public void Remove_WhenMultipleItemsExist_ShouldRemoveCorrectItem()
    {
        Trie.Add("a", "Item1");
        Trie.Add("b", "Item2");
        Trie.Add("c", "Item3");

        Trie.Remove(item => item == "Item2");

        Assert.Empty(Trie.Search("b"));
        Assert.Single(Trie.Search("a"));
        Assert.Single(Trie.Search("c"));
    }

    [Fact]
    public void Remove_WhenItemWithEmptyKeyExists_ShouldRemoveItem()
    {
        Trie.Add("", "Item1");

        Trie.Remove(item => item == "Item1");

        Assert.Empty(Trie.Search(""));
    }

    [Fact]
    public void Remove_WhenItemWithNullValueExists_ShouldRemoveItem()
    {
        Trie.Add("a", null!);

        Trie.Remove(item => item == null!);

        Assert.Empty(Trie.Search("a"));
    }
}
