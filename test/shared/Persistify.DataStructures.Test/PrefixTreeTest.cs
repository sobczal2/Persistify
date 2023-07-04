using Persistify.DataStructures.PrefixTree;
using System.Linq;
using Xunit;

namespace Persistify.DataStructures.Test;

public class PrefixTreeTest
{
    private readonly PrefixTree<ulong> _prefixTree;
    
    public PrefixTreeTest()
    {
        _prefixTree = new PrefixTree<ulong>();
    }
    
    [Fact]
    public void Add_SingleItem_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value = 1;
        
        // Act
        _prefixTree.Add(key, value);
        
        // Assert
        var result = _prefixTree.Search(key, false, true).Distinct().ToArray();
        Assert.Single(result);
        Assert.Equal(value, result[0]);
    }
    
    [Fact]
    public void Add_MultipleItems_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value1 = 1;
        const ulong value2 = 2;
        
        // Act
        _prefixTree.Add(key, value1);
        _prefixTree.Add(key, value2);
        
        // Assert
        var result = _prefixTree.Search(key, false, true).Distinct().ToArray();
        Assert.Equal(2, result.Length);
        Assert.Equal(value1, result[0]);
        Assert.Equal(value2, result[1]);
    }
    
    [Fact]
    public void Add_MultipleItemsWithSameValue_Success()
    {
        // Arrange
        const string key1 = "test1";
        const string key2 = "test2";
        const ulong value = 1;
        
        // Act
        _prefixTree.Add(key1, value);
        _prefixTree.Add(key2, value);
        
        // Assert
        var result1 = _prefixTree.Search(key1, false, true).Distinct().ToArray();
        Assert.Single(result1);
        Assert.Equal(value, result1[0]);
        
        var result2 = _prefixTree.Search(key2, false, true).Distinct().ToArray();
        Assert.Single(result2);
        Assert.Equal(value, result2[0]);
    }
    
    [Fact]
    public void Search_ExactMatch_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value = 1;
        _prefixTree.Add(key, value);
        
        // Act
        var result = _prefixTree.Search(key, false, true).Distinct().ToArray();
        
        // Assert
        Assert.Single(result);
        Assert.Equal(value, result[0]);
    }
    
    [Fact]
    public void Search_ExactMatchCaseSensitive_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value = 1;
        _prefixTree.Add(key, value);
        
        // Act
        var result = _prefixTree.Search(key, true, true).Distinct().ToArray();
        
        // Assert
        Assert.Single(result);
        Assert.Equal(value, result[0]);
    }
    
    [Fact]
    public void Search_ExactMatchCaseInsensitive_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value = 1;
        _prefixTree.Add(key, value);
        
        // Act
        var result = _prefixTree.Search(key.ToUpper(), false, true).Distinct().ToArray();
        
        // Assert
        Assert.Single(result);
        Assert.Equal(value, result[0]);
    }
    
    [Fact]
    public void Search_PartialMatch_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value = 1;
        _prefixTree.Add(key, value);
        
        // Act
        var result = _prefixTree.Search(key.Substring(0, 2), false, false).Distinct().ToArray();
        
        // Assert
        Assert.Single(result);
        Assert.Equal(value, result[0]);
    }
    
    [Fact]
    public void Search_PartialMatchCaseSensitive_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value = 1;
        _prefixTree.Add(key, value);
        
        // Act
        var result = _prefixTree.Search(key.Substring(0, 2), true, false).Distinct().ToArray();
        
        // Assert
        Assert.Single(result);
        Assert.Equal(value, result[0]);
    }
    
    [Fact]
    public void Search_PartialMatchCaseInsensitive_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value = 1;
        _prefixTree.Add(key, value);
        
        // Act
        var result = _prefixTree.Search(key.Substring(0, 2).ToUpper(), false, false).Distinct().ToArray();
        
        // Assert
        Assert.Single(result);
        Assert.Equal(value, result[0]);
    }
    
    [Fact]
    public void Search_NoMatch_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value = 1;
        _prefixTree.Add(key, value);
        
        // Act
        var result = _prefixTree.Search("nomatch", false, true).Distinct().ToArray();
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public void Search_NoMatchCaseSensitive_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value = 1;
        _prefixTree.Add(key, value);
        
        // Act
        var result = _prefixTree.Search("nomatch", true, true).Distinct().ToArray();
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public void Search_NoMatchCaseInsensitive_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value = 1;
        _prefixTree.Add(key, value);
        
        // Act
        var result = _prefixTree.Search("nomatch", false, true).Distinct().ToArray();
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public void Search_MultipleMatches_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value1 = 1;
        const ulong value2 = 2;
        _prefixTree.Add(key, value1);
        _prefixTree.Add(key, value2);
        
        // Act
        var result = _prefixTree.Search(key, false, true).Distinct().ToArray();
        
        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(value1, result[0]);
        Assert.Equal(value2, result[1]);
    }
    
    [Fact]
    public void Search_MultipleMatchesCaseSensitive_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value1 = 1;
        const ulong value2 = 2;
        _prefixTree.Add(key, value1);
        _prefixTree.Add(key, value2);
        
        // Act
        var result = _prefixTree.Search(key, true, true).Distinct().ToArray();
        
        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(value1, result[0]);
        Assert.Equal(value2, result[1]);
    }
    
    [Fact]
    public void Search_MultipleMatchesCaseInsensitive_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value1 = 1;
        const ulong value2 = 2;
        _prefixTree.Add(key, value1);
        _prefixTree.Add(key, value2);
        
        // Act
        var result = _prefixTree.Search(key.ToUpper(), false, true).Distinct().ToArray();
        
        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(value1, result[0]);
        Assert.Equal(value2, result[1]);
    }
    
    [Fact]
    public void Search_MultipleMatchesPartial_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value1 = 1;
        const ulong value2 = 2;
        _prefixTree.Add(key, value1);
        _prefixTree.Add(key, value2);
        
        // Act
        var result = _prefixTree.Search(key.Substring(0, 2), false, false).Distinct().ToArray();
        
        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(value1, result[0]);
        Assert.Equal(value2, result[1]);
    }
    
    [Fact]
    public void Search_MultipleMatchesPartialCaseSensitive_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value1 = 1;
        const ulong value2 = 2;
        _prefixTree.Add(key, value1);
        _prefixTree.Add(key, value2);
        
        // Act
        var result = _prefixTree.Search(key.Substring(0, 2), true, false).Distinct().ToArray();
        
        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(value1, result[0]);
        Assert.Equal(value2, result[1]);
    }
    
    [Fact]
    public void Search_MultipleMatchesPartialCaseInsensitive_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value1 = 1;
        const ulong value2 = 2;
        _prefixTree.Add(key, value1);
        _prefixTree.Add(key, value2);
        
        // Act
        var result = _prefixTree.Search(key.Substring(0, 2).ToUpper(), false, false).Distinct().ToArray();
        
        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(value1, result[0]);
        Assert.Equal(value2, result[1]);
    }
    
    [Fact]
    public void Remove_NoMatch_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value = 1;
        _prefixTree.Add(key, value);
        
        // Act
        _prefixTree.Remove(x => x == 2);
        
        // Assert
        Assert.Single(_prefixTree.Search(key, false, true).Distinct().ToArray());
    }
    
    [Fact]
    public void Remove_Match_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value = 1;
        _prefixTree.Add(key, value);
        
        // Act
        _prefixTree.Remove(x => x == value);
        
        // Assert
        Assert.Empty(_prefixTree.Search(key, false, true));
    }
    
    [Fact]
    public void Remove_MultipleMatches_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value1 = 1;
        const ulong value2 = 2;
        _prefixTree.Add(key, value1);
        _prefixTree.Add(key, value2);
        
        // Act
        _prefixTree.Remove(x => x == value1);
        
        // Assert
        Assert.Single(_prefixTree.Search(key, false, true).Distinct().ToArray());
    }
    
    [Fact]
    public void Remove_MultipleMatchesAll_Success()
    {
        // Arrange
        const string key = "test";
        const ulong value1 = 1;
        const ulong value2 = 2;
        _prefixTree.Add(key, value1);
        _prefixTree.Add(key, value2);
        
        // Act
        _prefixTree.Remove(x => x == value1);
        _prefixTree.Remove(x => x == value2);
        
        // Assert
        Assert.Empty(_prefixTree.Search(key, false, true).Distinct().ToArray());
    }
    
    [Fact]
    public void Remove_SingleMatch_RootNodeCleared()
    {
        // Arrange
        const string key = "test";
        const ulong value = 1;
        _prefixTree.Add(key, value);
        
        // Act
        _prefixTree.Remove(x => x == value);
        
        // Assert
        Assert.All(_prefixTree.GetRoot().Children, Assert.Null);
    }
}
