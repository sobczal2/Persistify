using System.Linq;
using FluentAssertions;
using Persistify.DataStructures.Test;
using Persistify.Management.Fts.Manager;
using Persistify.Management.Fts.Search;
using Persistify.Management.Fts.Token;
using Persistify.Management.Score;
using Persistify.Protos.Documents.Shared;
using Xunit;
using FtsQuery = Persistify.Management.Fts.Search.FtsQuery;

namespace Persistify.Management.Tests.Unit.Fts;

public class PrefixTreeFtsManagerTest
{
    private readonly IFtsManager _ftsManager;

    public PrefixTreeFtsManagerTest()
    {
        _ftsManager = new PrefixTreeFtsManager(new DefaultTokenizer(), new LinearScoreCalculator());
    }

    [Fact]
    public void Add_ShouldAddDocuments_WhenDocumentsAreFromSameTemplate()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "hello world" },
                new TextField { FieldName = "field2", Value = "hello world" }
            }
        };
        var document2 = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "hello world" },
                new TextField { FieldName = "field2", Value = "hello world" }
            }
        };

        // Act
        _ftsManager.Add(templateName, document1, 1);
        _ftsManager.Add(templateName, document2, 2);

        // Assert
        var prefixTree = _ftsManager.GetPrefixTree(templateName, "field1");
        prefixTree.Should().NotBeNull();
        var exactValues = prefixTree!.GetAllValues().Where(x => x.Flags == PrefixTreeValueFlags.Exact).ToList();
        exactValues.Should().HaveCount(4);
        exactValues.Should().Contain(x => x.DocumentId == 1);
        exactValues.Should().Contain(x => x.DocumentId == 2);
    }

    [Fact]
    public void Add_ShouldAddDocuments_WhenDocumentsAreFromDifferentTemplates()
    {
        // Arrange
        const string templateName1 = "template1";
        const string templateName2 = "template2";
        var document1 = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "hello world" },
                new TextField { FieldName = "field2", Value = "hello world" }
            }
        };
        var document2 = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "hello world" },
                new TextField { FieldName = "field2", Value = "hello world" }
            }
        };

        // Act
        _ftsManager.Add(templateName1, document1, 1);
        _ftsManager.Add(templateName2, document2, 2);

        // Assert
        var prefixTree1 = _ftsManager.GetPrefixTree(templateName1, "field1");
        prefixTree1.Should().NotBeNull();
        var exactValues1 = prefixTree1!.GetAllValues().Where(x => x.Flags == PrefixTreeValueFlags.Exact).ToList();
        exactValues1.Should().HaveCount(2);
        exactValues1.Should().Contain(x => x.DocumentId == 1);

        var prefixTree2 = _ftsManager.GetPrefixTree(templateName2, "field1");
        prefixTree2.Should().NotBeNull();
        var exactValues2 = prefixTree2!.GetAllValues().Where(x => x.Flags == PrefixTreeValueFlags.Exact).ToList();
        exactValues2.Should().HaveCount(2);
        exactValues2.Should().Contain(x => x.DocumentId == 2);
    }

    [Fact]
    public void Search_ShouldReturnEmptyCollection_WhenTemplateDoesNotExist()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "hello world" },
                new TextField { FieldName = "field2", Value = "hello world" }
            }
        };
        var document2 = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "hello world" },
                new TextField { FieldName = "field2", Value = "hello world" }
            }
        };
        _ftsManager.Add(templateName, document1, 1);
        _ftsManager.Add(templateName, document2, 2);

        // Act
        var result = _ftsManager.Search("template2",
            new FtsQuery { FieldName = "field1", Value = "hello", Exact = false, CaseSensitive = false });

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Search_ShouldReturnEmptyCollection_WhenFieldDoesNotExist()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "hello world" },
                new TextField { FieldName = "field2", Value = "hello world" }
            }
        };
        var document2 = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "hello world" },
                new TextField { FieldName = "field2", Value = "hello world" }
            }
        };
        _ftsManager.Add(templateName, document1, 1);
        _ftsManager.Add(templateName, document2, 2);

        // Act
        var result = _ftsManager.Search(templateName,
            new FtsQuery { FieldName = "field3", Value = "hello", Exact = false, CaseSensitive = false });

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Search_ShouldReturnEmptyCollection_WhenValueDoesNotExist()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "hello world" },
                new TextField { FieldName = "field2", Value = "hello world" }
            }
        };
        _ftsManager.Add(templateName, document, 1);

        // Act
        var result = _ftsManager.Search(templateName,
            new FtsQuery { FieldName = "field1", Value = "hello2", Exact = false, CaseSensitive = false });

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Search_ShouldReturnDocument_WhenValueIsPrefixAndExactIsFalse()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "hello world" },
                new TextField { FieldName = "field2", Value = "hello world" }
            }
        };
        _ftsManager.Add(templateName, document, 1);

        // Act
        var result = _ftsManager.Search(templateName,
            new FtsQuery { FieldName = "field1", Value = "wor", Exact = false, CaseSensitive = false }).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(x => x.DocumentId == 1);
    }

    [Fact]
    public void Search_ShouldReturnEmptyCollection_WhenValueIsPrefixAndExactIsTrue()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "hello world" },
                new TextField { FieldName = "field2", Value = "hello world" }
            }
        };
        _ftsManager.Add(templateName, document, 1);

        // Act
        var result = _ftsManager.Search(templateName,
            new FtsQuery { FieldName = "field1", Value = "wor", Exact = true, CaseSensitive = false }).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Search_ShouldReturnDocument_WhenValueIsExactAndExactIsTrue()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "hello world" },
                new TextField { FieldName = "field2", Value = "hello world" }
            }
        };
        _ftsManager.Add(templateName, document, 1);

        // Act
        var result = _ftsManager.Search(templateName,
            new FtsQuery { FieldName = "field1", Value = "world", Exact = true, CaseSensitive = false }).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(x => x.DocumentId == 1);
    }

    [Fact]
    public void Search_ShouldReturnDocument_WhenValueIsExactAndExactIsFalse()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "hello world" },
                new TextField { FieldName = "field2", Value = "hello world" }
            }
        };
        _ftsManager.Add(templateName, document, 1);

        // Act
        var result = _ftsManager.Search(templateName,
            new FtsQuery { FieldName = "field1", Value = "world", Exact = false, CaseSensitive = false }).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(x => x.DocumentId == 1);
    }

    [Fact]
    public void Search_ShouldReturnDocument_WhenValueIsSuffixAndExactIsFalse()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "hello world" },
                new TextField { FieldName = "field2", Value = "hello world" }
            }
        };
        _ftsManager.Add(templateName, document, 1);

        // Act
        var result = _ftsManager.Search(templateName,
            new FtsQuery { FieldName = "field1", Value = "ld", Exact = false, CaseSensitive = false }).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(x => x.DocumentId == 1);
    }

    [Fact]
    public void Search_ShouldReturnDocument_WhenValueIsSameCasingAndCaseSensitiveIsTrue()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "Hello World" },
                new TextField { FieldName = "field2", Value = "Hello World" }
            }
        };
        _ftsManager.Add(templateName, document, 1);

        // Act
        var result = _ftsManager.Search(templateName,
            new FtsQuery { FieldName = "field1", Value = "Hello", Exact = true, CaseSensitive = true }).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(x => x.DocumentId == 1);
    }

    [Fact]
    public void Search_ShouldReturnDocument_WhenValueIsSameCasingAndCaseSensitiveIsFalse()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "Hello World" },
                new TextField { FieldName = "field2", Value = "Hello World" }
            }
        };
        _ftsManager.Add(templateName, document, 1);

        // Act
        var result = _ftsManager.Search(templateName,
            new FtsQuery { FieldName = "field1", Value = "Hello", Exact = true, CaseSensitive = false }).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(x => x.DocumentId == 1);
    }

    [Fact]
    public void Search_ShouldReturnEmptyCollection_WhenValueIsDifferentCasingAndCaseSensitiveIsTrue()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "Hello World" },
                new TextField { FieldName = "field2", Value = "Hello World" }
            }
        };
        _ftsManager.Add(templateName, document, 1);

        // Act
        var result = _ftsManager.Search(templateName,
            new FtsQuery { FieldName = "field1", Value = "hello", Exact = true, CaseSensitive = true }).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Search_ShouldReturnDocument_WhenValueIsDifferentCasingAndCaseSensitiveIsFalse()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "Hello World" },
                new TextField { FieldName = "field2", Value = "Hello World" }
            }
        };
        _ftsManager.Add(templateName, document, 1);

        // Act
        var result = _ftsManager.Search(templateName,
            new FtsQuery { FieldName = "field1", Value = "hello", Exact = true, CaseSensitive = false }).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(x => x.DocumentId == 1);
    }

    [Fact]
    public void Delete_ShouldDeleteDocument_WhenDocumentExists()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "hello world" },
                new TextField { FieldName = "field2", Value = "hello world" }
            }
        };
        _ftsManager.Add(templateName, document, 1);

        // Act
        _ftsManager.Delete(templateName, 1);

        // Assert
        var result = _ftsManager.Search(templateName,
            new FtsQuery { FieldName = "field1", Value = "hello", Exact = true, CaseSensitive = false }).ToList();
        result.Should().BeEmpty();
    }

    [Fact]
    public void Delete_ShouldNotDeleteDocument_WhenDocumentDoesNotExist()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            TextFields = new[]
            {
                new TextField { FieldName = "field1", Value = "hello world" },
                new TextField { FieldName = "field2", Value = "hello world" }
            }
        };
        _ftsManager.Add(templateName, document, 1);

        // Act
        _ftsManager.Delete(templateName, 2);

        // Assert
        var result = _ftsManager.Search(templateName,
            new FtsQuery { FieldName = "field1", Value = "hello", Exact = true, CaseSensitive = false }).ToList();
        result.Should().HaveCount(1);
    }
}
