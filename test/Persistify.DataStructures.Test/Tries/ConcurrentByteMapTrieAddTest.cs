using System.Linq;
using Xunit;

namespace Persistify.DataStructures.Test.Tries;

public class ConcurrentByteMapTrieAddTest : TrieTestBase
{
    [Fact]
    public void Add_WhenTreeIsEmpty_ShouldAddItemToRoot()
    {
        Trie.Add("a", "Item1");

        var root = Trie.GetRoot();

        Assert.Equal("Item1", root?.GetChildren()[10]?.GetItems()?.FirstOrDefault());
    }

    [Fact]
    public void Add_WhenItemIsAdded_ShouldContainItem()
    {
        Trie.Add("b", "Item2");

        Assert.Equal("Item2", Trie.Search("b", false, false).FirstOrDefault());
    }

    [Fact]
    public void Add_WhenMultipleItemsAreAdded_ShouldContainAllItems()
    {
        Trie.Add("a", "Item1");
        Trie.Add("b", "Item2");
        Trie.Add("c", "Item3");

        Assert.Equal("Item1", Trie.Search("a", false, false).FirstOrDefault());
        Assert.Equal("Item2", Trie.Search("b", false, false).FirstOrDefault());
        Assert.Equal("Item3", Trie.Search("c", false, false).FirstOrDefault());
    }

    [Fact]
    public void Add_WhenItemWithEmptyKeyIsAdded_ShouldContainItem()
    {
        Trie.Add("", "Item1");

        Assert.Equal("Item1", Trie.Search("", false, false).FirstOrDefault());
    }

    [Fact]
    public void Add_WhenItemWithNullValueIsAdded_ShouldContainItem()
    {
        Trie.Add("a", null!);

        Assert.Null(Trie.Search("a", false, false).FirstOrDefault());
    }
}