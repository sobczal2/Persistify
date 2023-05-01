using System.Linq;
using Xunit;

namespace Persistify.DataStructures.Test.Tries;

public class ConcurrentByteMapTrieSearchTest : TrieTestBase
{
    [Fact]
    public void Search_WhenTreeIsEmpty_ShouldReturnEmptyList()
    {
        var result = Trie.Search("a");

        Assert.Empty(result);
    }

    [Fact]
    public void Search_WhenItemExists_ShouldReturnItem()
    {
        Trie.Add("b", "Item2");

        Assert.Equal("Item2", Trie.Search("b").First());
    }

    [Fact]
    public void Search_WhenMultipleItemsExist_ShouldReturnCorrectItem()
    {
        Trie.Add("a", "Item1");
        Trie.Add("b", "Item2");
        Trie.Add("c", "Item3");

        Assert.Equal("Item2", Trie.Search("b").First());
    }

    [Fact]
    public void Search_WhenItemWithEmptyKeyExists_ShouldReturnItem()
    {
        Trie.Add("", "Item1");

        Assert.Equal("Item1", Trie.Search("").First());
    }

    [Fact]
    public void Search_WhenItemWithNullValueExists_ShouldReturnNull()
    {
        Trie.Add("a", null!);

        Assert.Null(Trie.Search("a").First());
    }
    
    [Fact]
    public void Search_WhenLongKeyIsUsed_ShouldReturnCorrectItem()
    {
        Trie.Add("longkey", "Item1");

        Assert.Equal("Item1", Trie.Search("longkey").First());
    }

    [Fact]
    public void Search_WhenKeyWithWildcardIsUsed_ShouldReturnAllMatchingItems()
    {
        Trie.Add("a1", "Item1");
        Trie.Add("a2", "Item2");
        Trie.Add("b1", "Item3");

        var results = Trie.Search("a$").ToList();

        Assert.Equal(2, results.Count);
        Assert.Contains("Item1", results);
        Assert.Contains("Item2", results);
    }

    [Fact]
    public void Search_WhenPrefixKeyIsUsed_ShouldReturnAllMatchingItems()
    {
        Trie.Add("prefix1", "Item1");
        Trie.Add("prefix2", "Item2");
        Trie.Add("otherprefix1", "Item3");

        var results = Trie.Search("prefix").ToList();

        Assert.Equal(2, results.Count);
        Assert.Contains("Item1", results);
        Assert.Contains("Item2", results);
    }

    [Fact]
    public void Search_WhenActualKeyIsUsed_ShouldReturnSingleMatchingItem()
    {
        Trie.Add("actualkey", "Item1");
        Trie.Add("actualkey1", "Item2");
        Trie.Add("actualkey2", "Item3");

        var result = Trie.Search("actualkey").First();

        Assert.Equal("Item1", result);
    }
}