using System.Linq;
using Persistify.DataStructures.MultiTargetTries.MultitargetTrieByteTranslationTrie;
using Persistify.DataStructures.MultiTargetTries.MultitargetTrieByteTranslationTrie.Mappers;
using Xunit;

namespace Persistify.DataStructures.Test.MultiTargetTries;

public class MultiTargetTrieTests
{
    [Fact]
    public void Add_ValidItem_AddsItemToTrie()
    {
        // Arrange
        var singleTargetMapper = new StandardCaseSensitiveSingleTargetMapper();
        var trie = new MultiTargetTrie<long>(singleTargetMapper.AlphabetSize);
        const string key = "test";
        const int item = 1234;

        // Act
        trie.Add(key, item, singleTargetMapper);

        // Assert
        Assert.True(trie.Contains(key, new StandardCaseSensitiveMultiTargetMapper()));
    }

    [Fact]
    public void Search_PrefixQuery_ReturnsExpectedItems()
    {
        // Arrange
        var singleTargetMapper = new StandardCaseSensitiveSingleTargetMapper();
        var trie = new MultiTargetTrie<long>(singleTargetMapper.AlphabetSize);
        trie.Add("apple", 1337, singleTargetMapper);
        trie.Add("application", 31337, singleTargetMapper);
        var multiTargetMapper = new StandardCaseSensitiveMultiTargetMapper();
        var query = "app";

        // Act
        var results = trie.Search(query, multiTargetMapper).ToList();

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Contains(1337, results);
        Assert.Contains(31337, results);
    }

    [Fact]
    public void Search_FullKeyQuery_ReturnsExpectedItems()
    {
        // Arrange
        var singleTargetMapper = new StandardCaseSensitiveSingleTargetMapper();
        var trie = new MultiTargetTrie<long>(singleTargetMapper.AlphabetSize);
        trie.Add("apple", 1337, singleTargetMapper);
        trie.Add("application", 31337, singleTargetMapper);
        var multiTargetMapper = new StandardCaseSensitiveMultiTargetMapper();
        var query = "application";

        // Act
        var results = trie.Search(query, multiTargetMapper).ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains(31337, results);
    }

    [Fact]
    public void Contains_ValidKey_ReturnsTrue()
    {
        // Arrange
        var singleTargetMapper = new StandardCaseSensitiveSingleTargetMapper();
        var trie = new MultiTargetTrie<long>(singleTargetMapper.AlphabetSize);
        const string key = "example";
        const int item = 1234;
        trie.Add(key, item, singleTargetMapper);
        var multiTargetMapper = new StandardCaseSensitiveMultiTargetMapper();

        // Act
        var result = trie.Contains(key, multiTargetMapper);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_InvalidKey_ReturnsFalse()
    {
        // Arrange
        var singleTargetMapper = new StandardCaseSensitiveSingleTargetMapper();
        var trie = new MultiTargetTrie<long>(singleTargetMapper.AlphabetSize);
        const string key = "invalid";
        var multiTargetMapper = new StandardCaseSensitiveMultiTargetMapper();

        // Act
        var result = trie.Contains(key, multiTargetMapper);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Contains_ValidPrefix_ReturnsTrue()
    {
        // Arrange
        var singleTargetMapper = new StandardCaseSensitiveSingleTargetMapper();
        var trie = new MultiTargetTrie<long>(singleTargetMapper.AlphabetSize);
        trie.Add("apple", 1337, singleTargetMapper);
        trie.Add("application", 31337, singleTargetMapper);
        var multiTargetMapper = new StandardCaseSensitiveMultiTargetMapper();
        const string prefix = "app";
        // Act
        var result = trie.Contains(prefix, multiTargetMapper);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_InvalidPrefix_ReturnsFalse()
    {
        // Arrange
        var singleTargetMapper = new StandardCaseSensitiveSingleTargetMapper();
        var trie = new MultiTargetTrie<long>(singleTargetMapper.AlphabetSize);
        trie.Add("apple", 1337, singleTargetMapper);
        trie.Add("application", 31337, singleTargetMapper);
        var multiTargetMapper = new StandardCaseSensitiveMultiTargetMapper();
        const string prefix = "banana";

        // Act
        var result = trie.Contains(prefix, multiTargetMapper);

        // Assert
        Assert.False(result);
    }
}