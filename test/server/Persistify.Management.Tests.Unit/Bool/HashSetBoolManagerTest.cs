using System.Linq;
using FluentAssertions;
using Persistify.Management.Bool.Manager;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using Xunit;
using BoolQuery = Persistify.Management.Bool.Search.BoolQuery;

namespace Persistify.Management.Tests.Unit.Bool;

public class HashSetBoolManagerTest
{
    private readonly IBoolManager _sut;

    public HashSetBoolManagerTest()
    {
        _sut = new HashSetBoolManager(new LinearScoreCalculator());
    }

    [Fact]
    public void Add_ShouldAddDocuments_WhenDocumentsAreFromSameTemplate()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            BoolFields =
            {
                new BoolField { FieldName = "field1", Value = true },
                new BoolField { FieldName = "field2", Value = false }
            }
        };
        var document2 = new Document
        {
            BoolFields =
            {
                new BoolField { FieldName = "field1", Value = false },
                new BoolField { FieldName = "field2", Value = true }
            }
        };

        // Act
        _sut.Add(templateName, document1, 1);
        _sut.Add(templateName, document2, 2);

        // Assert
        var trueHashSet = _sut.GetHashSet(templateName, "field1", true);
        trueHashSet.Should().HaveCount(1);
        trueHashSet.Should().Contain(1);

        var falseHashSet = _sut.GetHashSet(templateName, "field1", false);
        falseHashSet.Should().HaveCount(1);
        falseHashSet.Should().Contain(2);

        trueHashSet = _sut.GetHashSet(templateName, "field2", true);
        trueHashSet.Should().HaveCount(1);
        trueHashSet.Should().Contain(2);

        falseHashSet = _sut.GetHashSet(templateName, "field2", false);
        falseHashSet.Should().HaveCount(1);
        falseHashSet.Should().Contain(1);
    }

    [Fact]
    public void Add_ShouldAddDocuments_WhenDocumentsAreFromDifferentTemplates()
    {
        // Arrange
        const string templateName1 = "template1";
        var document1 = new Document
        {
            BoolFields =
            {
                new BoolField { FieldName = "field1", Value = true },
                new BoolField { FieldName = "field2", Value = false }
            }
        };
        const string templateName2 = "template2";
        var document2 = new Document
        {
            BoolFields =
            {
                new BoolField { FieldName = "field1", Value = false },
                new BoolField { FieldName = "field2", Value = true }
            }
        };

        // Act
        _sut.Add(templateName1, document1, 1);
        _sut.Add(templateName2, document2, 2);

        // Assert
        var trueHashSet = _sut.GetHashSet(templateName1, "field1", true);
        trueHashSet.Should().HaveCount(1);
        trueHashSet.Should().Contain(1);

        var falseHashSet = _sut.GetHashSet(templateName1, "field1", false);
        falseHashSet.Should().BeNull();

        trueHashSet = _sut.GetHashSet(templateName1, "field2", true);
        trueHashSet.Should().BeNull();

        falseHashSet = _sut.GetHashSet(templateName1, "field2", false);
        falseHashSet.Should().HaveCount(1);
        falseHashSet.Should().Contain(1);

        trueHashSet = _sut.GetHashSet(templateName2, "field1", true);
        trueHashSet.Should().BeNull();

        falseHashSet = _sut.GetHashSet(templateName2, "field1", false);
        falseHashSet.Should().HaveCount(1);
        falseHashSet.Should().Contain(2);

        trueHashSet = _sut.GetHashSet(templateName2, "field2", true);
        trueHashSet.Should().HaveCount(1);
        trueHashSet.Should().Contain(2);

        falseHashSet = _sut.GetHashSet(templateName2, "field2", false);
        falseHashSet.Should().BeNull();
    }

    [Fact]
    public void Search_ShouldReturnEmptyCollection_WhenTemplateDoesNotExist()
    {
        // Arrange
        const string templateName = "template";
        var query = new BoolQuery { FieldName = "field1", Value = true };

        // Act
        var result = _sut.Search(templateName, query);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Search_ShouldReturnEmptyCollection_WhenFieldDoesNotExist()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            BoolFields =
            {
                new BoolField { FieldName = "field1", Value = true },
                new BoolField { FieldName = "field2", Value = false }
            }
        };
        _sut.Add(templateName, document, 1);
        var query = new BoolQuery { FieldName = "field3", Value = true };

        // Act
        var result = _sut.Search(templateName, query);

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
            BoolFields =
            {
                new BoolField { FieldName = "field1", Value = true },
                new BoolField { FieldName = "field2", Value = false }
            }
        };
        _sut.Add(templateName, document, 1);
        var query = new BoolQuery { FieldName = "field1", Value = false };

        // Act
        var result = _sut.Search(templateName, query);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Search_ShouldReturnDocument_WhenDocumentExists()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            BoolFields =
            {
                new BoolField { FieldName = "field1", Value = true },
                new BoolField { FieldName = "field2", Value = false }
            }
        };
        var document2 = new Document
        {
            BoolFields =
            {
                new BoolField { FieldName = "field1", Value = false },
                new BoolField { FieldName = "field2", Value = true }
            }
        };
        _sut.Add(templateName, document1, 1);
        _sut.Add(templateName, document2, 2);
        var query = new BoolQuery { FieldName = "field1", Value = true };

        // Act
        var result = _sut.Search(templateName, query).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(x => x.DocumentId == 1);
    }

    [Fact]
    public void Search_ShouldReturnDocuments_WhenDocumentsExist()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            BoolFields =
            {
                new BoolField { FieldName = "field1", Value = false },
                new BoolField { FieldName = "field2", Value = true }
            }
        };
        var document2 = new Document
        {
            BoolFields =
            {
                new BoolField { FieldName = "field1", Value = false },
                new BoolField { FieldName = "field2", Value = true }
            }
        };
        _sut.Add(templateName, document1, 1);
        _sut.Add(templateName, document2, 2);
        var query = new BoolQuery { FieldName = "field2", Value = true };

        // Act
        var result = _sut.Search(templateName, query).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.DocumentId == 1);
        result.Should().Contain(x => x.DocumentId == 2);
    }

    [Fact]
    public void Delete_ShouldDeleteDocument_WhenDocumentExists()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            BoolFields =
            {
                new BoolField { FieldName = "field1", Value = true },
                new BoolField { FieldName = "field2", Value = false }
            }
        };
        _sut.Add(templateName, document, 1);

        // Act
        _sut.Delete(templateName, 1);

        // Assert
        var trueHashSet = _sut.GetHashSet(templateName, "field1", true);
        trueHashSet.Should().BeNull();

        var falseHashSet = _sut.GetHashSet(templateName, "field1", false);
        falseHashSet.Should().BeNull();

        trueHashSet = _sut.GetHashSet(templateName, "field2", true);
        trueHashSet.Should().BeNull();

        falseHashSet = _sut.GetHashSet(templateName, "field2", false);
        falseHashSet.Should().BeNull();
    }

    [Fact]
    public void Delete_ShouldNotDeleteDocument_WhenDocumentDoesNotExist()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            BoolFields =
            {
                new BoolField { FieldName = "field1", Value = true },
                new BoolField { FieldName = "field2", Value = false }
            }
        };
        _sut.Add(templateName, document, 1);

        // Act
        _sut.Delete(templateName, 2);

        // Assert
        var trueHashSet = _sut.GetHashSet(templateName, "field1", true);
        trueHashSet.Should().HaveCount(1);
        trueHashSet.Should().Contain(1);

        var falseHashSet = _sut.GetHashSet(templateName, "field1", false);
        falseHashSet.Should().BeNull();

        trueHashSet = _sut.GetHashSet(templateName, "field2", true);
        trueHashSet.Should().BeNull();

        falseHashSet = _sut.GetHashSet(templateName, "field2", false);
        falseHashSet.Should().HaveCount(1);
        falseHashSet.Should().Contain(1);
    }
}
