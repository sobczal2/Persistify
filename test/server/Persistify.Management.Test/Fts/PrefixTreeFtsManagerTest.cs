using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Persistify.DataStructures.Test;
using Persistify.Helpers.Strings;
using Persistify.Management.Fts.Manager;
using Persistify.Management.Fts.Search;
using Persistify.Management.Fts.Token;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using Xunit;
using FtsQuery = Persistify.Management.Fts.Search.FtsQuery;

namespace Persistify.Management.Test.Fts;

public class PrefixTreeFtsManagerTest
{
    private class TestTokenizer : ITokenizer
    {
        public ISet<string> Tokenize(string value)
        {
            return new HashSet<string>(value.Split(' '));
        }

        public ISet<string> TokenizeWithWildcards(string value)
        {
            return new HashSet<string>(value.Split(' '));
        }
    }

    private readonly IFtsManager _ftsManager;

    public PrefixTreeFtsManagerTest()
    {
        _ftsManager = new PrefixTreeFtsManager(new TestTokenizer(), new LinearScoreCalculator());
    }

    [Fact]
    public async Task AddAsync_SingleFieldSingleTokenSingleDocument_DocumentAdded()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value";
        var document = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId = 1UL;

        // Act
        await _ftsManager.AddAsync(templateName, document, documentId);

        // Assert
        var prefixTree = _ftsManager.GetPrefixTree(templateName, fieldName);
        Assert.NotNull(prefixTree);
        Assert.Single(prefixTree.GetAllValues().Where(x => x.Flags == PrefixTreeValueFlags.Exact));
        Assert.Equal(StringHelpers.GetSuffixes(value).Count(), prefixTree.GetAllValues().Count());
    }

    [Fact]
    public async Task AddAsync_SingleFieldSingleTokenMultipleDocuments_DocumentsAdded()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value";
        var document1 = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId1 = 1UL;
        var document2 = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId2 = 2UL;

        // Act
        await _ftsManager.AddAsync(templateName, document1, documentId1);
        await _ftsManager.AddAsync(templateName, document2, documentId2);

        // Assert
        var prefixTree = _ftsManager.GetPrefixTree(templateName, fieldName);
        Assert.NotNull(prefixTree);
        Assert.Equal(2, prefixTree.GetAllValues().Count(x => x.Flags == PrefixTreeValueFlags.Exact));
        Assert.Equal(StringHelpers.GetSuffixes(value).Count() * 2, prefixTree.GetAllValues().Count());
    }

    [Fact]
    public async Task AddAsync_SingleFieldMultipleTokensSingleDocument_DocumentAdded()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value1 value2";
        var document = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId = 1UL;

        // Act
        await _ftsManager.AddAsync(templateName, document, documentId);

        // Assert
        var prefixTree = _ftsManager.GetPrefixTree(templateName, fieldName);
        Assert.NotNull(prefixTree);
        Assert.Equal(2, prefixTree.GetAllValues().Count(x => x.Flags == PrefixTreeValueFlags.Exact));
        Assert.Equal(
            StringHelpers.GetSuffixes(value.Split(' ')[0]).Count() +
            StringHelpers.GetSuffixes(value.Split(' ')[0]).Count(), prefixTree.GetAllValues().Count());
    }

    [Fact]
    public async Task AddAsync_MultipleFieldsSingleTokenSingleDocument_DocumentAdded()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName1 = "field1";
        const string fieldName2 = "field2";
        const string value = "value";
        var document = new Document()
        {
            TextFields =
            {
                new TextField() { FieldName = fieldName1, Value = value },
                new TextField() { FieldName = fieldName2, Value = value }
            }
        };
        const ulong documentId = 1UL;

        // Act
        await _ftsManager.AddAsync(templateName, document, documentId);

        // Assert
        var prefixTree1 = _ftsManager.GetPrefixTree(templateName, fieldName1);
        Assert.NotNull(prefixTree1);
        Assert.Single(prefixTree1.GetAllValues().Where(x => x.Flags == PrefixTreeValueFlags.Exact));
        Assert.Equal(StringHelpers.GetSuffixes(value).Count(), prefixTree1.GetAllValues().Count());
        var prefixTree2 = _ftsManager.GetPrefixTree(templateName, fieldName2);
        Assert.NotNull(prefixTree2);
        Assert.Single(prefixTree2.GetAllValues().Where(x => x.Flags == PrefixTreeValueFlags.Exact));
        Assert.Equal(StringHelpers.GetSuffixes(value).Count(), prefixTree2.GetAllValues().Count());
    }

    [Fact]
    public async Task SearchAsync_ExactCaseSensitive_FoundCorrectly()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value";
        var document = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId = 1UL;
        await _ftsManager.AddAsync(templateName, document, documentId);
        var query = new FtsQuery() { FieldName = fieldName, Value = value, Exact = true, CaseSensitive = true };

        // Act
        var result = await _ftsManager.SearchAsync(templateName, query);

        // Assert
        Assert.Single(result);
        Assert.Equal(documentId, result.First().Id);
    }
    
    [Fact]
    public async Task SearchAsync_ExactCaseInsensitive_FoundCorrectly()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value";
        var document = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId = 1UL;
        await _ftsManager.AddAsync(templateName, document, documentId);
        var query = new FtsQuery() { FieldName = fieldName, Value = value.ToUpper(), Exact = true, CaseSensitive = false };

        // Act
        var result = await _ftsManager.SearchAsync(templateName, query);

        // Assert
        Assert.Single(result);
        Assert.Equal(documentId, result.First().Id);
    }
    
    [Fact]
    public async Task SearchAsync_PrefixCaseSensitive_FoundCorrectly()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value";
        var document = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId = 1UL;
        await _ftsManager.AddAsync(templateName, document, documentId);
        var query = new FtsQuery() { FieldName = fieldName, Value = value[..3], Exact = false, CaseSensitive = true };

        // Act
        var result = await _ftsManager.SearchAsync(templateName, query);

        // Assert
        Assert.Single(result);
        Assert.Equal(documentId, result.First().Id);
    }
    
    [Fact]
    public async Task SearchAsync_SuffixCaseSensitive_FoundCorrectly()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value";
        var document = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId = 1UL;
        await _ftsManager.AddAsync(templateName, document, documentId);
        var query = new FtsQuery() { FieldName = fieldName, Value = value[2..], Exact = false, CaseSensitive = true };

        // Act
        var result = await _ftsManager.SearchAsync(templateName, query);

        // Assert
        Assert.Single(result);
        Assert.Equal(documentId, result.First().Id);
    }
    
    [Fact]
    public async Task SearchAsync_PrefixCaseInsensitive_FoundCorrectly()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value";
        var document = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId = 1UL;
        await _ftsManager.AddAsync(templateName, document, documentId);
        var query = new FtsQuery() { FieldName = fieldName, Value = value[..3].ToUpper(), Exact = false, CaseSensitive = false };

        // Act
        var result = await _ftsManager.SearchAsync(templateName, query);

        // Assert
        Assert.Single(result);
        Assert.Equal(documentId, result.First().Id);
    }
    
    [Fact]
    public async Task SearchAsync_SuffixCaseInsensitive_FoundCorrectly()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value";
        var document = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId = 1UL;
        await _ftsManager.AddAsync(templateName, document, documentId);
        var query = new FtsQuery() { FieldName = fieldName, Value = value[2..].ToUpper(), Exact = false, CaseSensitive = false };

        // Act
        var result = await _ftsManager.SearchAsync(templateName, query);

        // Assert
        Assert.Single(result);
        Assert.Equal(documentId, result.First().Id);
    }
    
    [Fact]
    public async Task SearchAsync_PrefixCaseSensitive_NotFound()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value";
        var document = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId = 1UL;
        await _ftsManager.AddAsync(templateName, document, documentId);
        var query = new FtsQuery() { FieldName = fieldName, Value = value[..2].ToUpper(), Exact = false, CaseSensitive = true };

        // Act
        var result = await _ftsManager.SearchAsync(templateName, query);

        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task SearchAsync_SuffixCaseSensitive_NotFound()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value";
        var document = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId = 1UL;
        await _ftsManager.AddAsync(templateName, document, documentId);
        var query = new FtsQuery() { FieldName = fieldName, Value = value[2..].ToUpper(), Exact = false, CaseSensitive = true };

        // Act
        var result = await _ftsManager.SearchAsync(templateName, query);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_ExactCaseInsensitive_NotFound()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value";
        var document = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId = 1UL;
        await _ftsManager.AddAsync(templateName, document, documentId);
        var query = new FtsQuery() { FieldName = fieldName, Value = value[..2].ToUpper(), Exact = true, CaseSensitive = false };

        // Act
        var result = await _ftsManager.SearchAsync(templateName, query);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_ExactCaseSensitive_NotFound()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value";
        var document = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId = 1UL;
        await _ftsManager.AddAsync(templateName, document, documentId);
        var query = new FtsQuery() { FieldName = fieldName, Value = value.ToUpper(), Exact = true, CaseSensitive = true };

        // Act
        var result = await _ftsManager.SearchAsync(templateName, query);

        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task SearchAsync_PrefixCaseInsensitive_NotFound()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value";
        var document = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId = 1UL;
        await _ftsManager.AddAsync(templateName, document, documentId);
        var query = new FtsQuery() { FieldName = fieldName, Value = "smth", Exact = false, CaseSensitive = false };

        // Act
        var result = await _ftsManager.SearchAsync(templateName, query);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_SuffixCaseInsensitive_NotFound()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value";
        var document = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId = 1UL;
        await _ftsManager.AddAsync(templateName, document, documentId);
        var query = new FtsQuery() { FieldName = fieldName, Value = "smth", Exact = false, CaseSensitive = false };

        // Act
        var result = await _ftsManager.SearchAsync(templateName, query);

        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task DeleteAsync_SingleDocument_Deleted()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value";
        var document = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId = 1UL;

        await _ftsManager.AddAsync(templateName, document, documentId);

        // Act
        await _ftsManager.DeleteAsync(templateName, documentId);

        // Assert
        var result = await _ftsManager.SearchAsync(templateName, new FtsQuery() { FieldName = fieldName, Value = value, Exact = true, CaseSensitive = true });
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task DeleteAsync_MultipleDocuments_Deleted()
    {
        // Arrange
        const string templateName = "template";
        const string fieldName = "field";
        const string value = "value";

        var document1 = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId1 = 1UL;

        var document2 = new Document() { TextFields = { new TextField() { FieldName = fieldName, Value = value } } };
        const ulong documentId2 = 2UL;

        await _ftsManager.AddAsync(templateName, document1, documentId1);
        await _ftsManager.AddAsync(templateName, document2, documentId2);

        // Act
        await _ftsManager.DeleteAsync(templateName, documentId1);
        await _ftsManager.DeleteAsync(templateName, documentId2);

        // Assert
        var result = await _ftsManager.SearchAsync(templateName, new FtsQuery() { FieldName = fieldName, Value = value, Exact = true, CaseSensitive = true });
        Assert.Empty(result);
    }
}
