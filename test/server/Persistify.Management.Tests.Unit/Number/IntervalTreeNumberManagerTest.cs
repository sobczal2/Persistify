using FluentAssertions;
using Persistify.DataStructures.Test;
using Persistify.Management.Number.Manager;
using Persistify.Management.Score;
using Persistify.Protos.Documents;
using Persistify.Protos.Documents.Shared;
using Xunit;
using NumberQuery = Persistify.Management.Number.Search.NumberQuery;

namespace Persistify.Management.Tests.Unit.Number;

public class IntervalTreeNumberManagerTest
{
    private readonly INumberManager _sut;

    public IntervalTreeNumberManagerTest()
    {
        _sut = new IntervalTreeNumberManager(new LinearScoreCalculator());
    }

    [Fact]
    public void Add_ShouldAddDocuments_WhenDocumentsAreFromSameTemplate()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 1 },
                new NumberField { FieldName = "field2", Value = 2 }
            }
        };
        var document2 = new Document
        {
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 3 },
                new NumberField { FieldName = "field2", Value = 4 }
            }
        };

        // Act
        _sut.Add(templateName, document1, 1);
        _sut.Add(templateName, document2, 2);

        // Assert
        var intervalTree = _sut.GetIntervalTree(templateName, "field1");
        intervalTree.Should().NotBeNull();
        var values = intervalTree!.GetAllValues();
        values.Should().HaveCount(2);
        values.Should().Contain(x => x.DocumentId == 1);
        values.Should().Contain(x => x.DocumentId == 1);

        intervalTree = _sut.GetIntervalTree(templateName, "field2");
        intervalTree.Should().NotBeNull();
        values = intervalTree!.GetAllValues();
        values.Should().HaveCount(2);
        values.Should().Contain(x => x.DocumentId == 2);
        values.Should().Contain(x => x.DocumentId == 2);
    }

    [Fact]
    public void Add_ShouldAddDocuments_WhenDocumentsAreFromDifferentTemplates()
    {
        // Arrange
        const string templateName1 = "template1";
        const string templateName2 = "template2";
        var document1 = new Document
        {
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 1 },
                new NumberField { FieldName = "field2", Value = 2 }
            }
        };
        var document2 = new Document
        {
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 3 },
                new NumberField { FieldName = "field2", Value = 4 }
            }
        };

        // Act
        _sut.Add(templateName1, document1, 1);
        _sut.Add(templateName2, document2, 2);

        // Assert
        var intervalTree = _sut.GetIntervalTree(templateName1, "field1");
        intervalTree.Should().NotBeNull();
        var values = intervalTree!.GetAllValues();
        values.Should().HaveCount(1);
        values.Should().Contain(x => x.DocumentId == 1);

        intervalTree = _sut.GetIntervalTree(templateName1, "field2");
        intervalTree.Should().NotBeNull();
        values = intervalTree!.GetAllValues();
        values.Should().HaveCount(1);
        values.Should().Contain(x => x.DocumentId == 1);

        intervalTree = _sut.GetIntervalTree(templateName2, "field1");
        intervalTree.Should().NotBeNull();
        values = intervalTree!.GetAllValues();
        values.Should().HaveCount(1);
        values.Should().Contain(x => x.DocumentId == 2);

        intervalTree = _sut.GetIntervalTree(templateName2, "field2");
        intervalTree.Should().NotBeNull();
        values = intervalTree!.GetAllValues();
        values.Should().HaveCount(1);
        values.Should().Contain(x => x.DocumentId == 2);
    }

    [Fact]
    public void Search_ShouldReturnEmptyCollection_WhenTemplateDoesNotExist()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 1 },
                new NumberField { FieldName = "field2", Value = 2 }
            }
        };
        _sut.Add(templateName, document, 1);

        // Act
        var result = _sut.Search("template2", new NumberQuery { FieldName = "field1", MinValue = 0, MaxValue = 10 });

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
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 1 },
                new NumberField { FieldName = "field2", Value = 2 }
            }
        };
        _sut.Add(templateName, document, 1);

        // Act
        var result = _sut.Search(templateName, new NumberQuery { FieldName = "field3", MinValue = 0, MaxValue = 10 });

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Search_ShouldReturnEmptyCollection_WhenMinValueIsGreaterThanMaxValue()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 1 },
                new NumberField { FieldName = "field2", Value = 2 }
            }
        };
        _sut.Add(templateName, document, 1);

        // Act
        var result = _sut.Search(templateName, new NumberQuery { FieldName = "field1", MinValue = 10, MaxValue = 0 });

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Search_ShouldReturnEmptyCollection_WhenDocumentValueIsGreaterThanMaxValue()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 1 },
                new NumberField { FieldName = "field2", Value = 2 }
            }
        };
        _sut.Add(templateName, document, 1);

        // Act
        var result = _sut.Search(templateName, new NumberQuery { FieldName = "field1", MinValue = 0, MaxValue = 0 });

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Search_ShouldReturnEmptyCollection_WhenDocumentValueIsLessThanMinValue()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 1 },
                new NumberField { FieldName = "field2", Value = 2 }
            }
        };
        _sut.Add(templateName, document, 1);

        // Act
        var result = _sut.Search(templateName, new NumberQuery { FieldName = "field1", MinValue = 2, MaxValue = 2 });

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Search_ShouldReturnDocument_WhenDocumentValueIsEqualToMinValueAndMaxValue()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 1 },
                new NumberField { FieldName = "field2", Value = 2 }
            }
        };
        _sut.Add(templateName, document, 1);

        // Act
        var result = _sut.Search(templateName, new NumberQuery { FieldName = "field1", MinValue = 1, MaxValue = 1 });

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(x => x.DocumentId == 1);
    }

    [Fact]
    public void Search_ShouldReturnDocument_WhenDocumentValueIsGreaterThanMinValueAndLessThanMaxValue()
    {
        // Arrange
        const string templateName = "template";
        var document = new Document
        {
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 1 },
                new NumberField { FieldName = "field2", Value = 3 }
            }
        };
        _sut.Add(templateName, document, 1);

        // Act
        var result = _sut.Search(templateName, new NumberQuery { FieldName = "field1", MinValue = 0, MaxValue = 2 });

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(x => x.DocumentId == 1);
    }

    [Fact]
    public void Search_ShouldReturnDocuments_WhenDocumentValueIsGreaterThanMinValueAndEqualToMaxValue()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 1 },
                new NumberField { FieldName = "field2", Value = 3 }
            }
        };
        _sut.Add(templateName, document1, 1);

        var document2 = new Document
        {
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 2 },
                new NumberField { FieldName = "field2", Value = 4 }
            }
        };
        _sut.Add(templateName, document2, 2);

        // Act
        var result = _sut.Search(templateName, new NumberQuery { FieldName = "field1", MinValue = 0, MaxValue = 2 });

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.DocumentId == 1);
        result.Should().Contain(x => x.DocumentId == 2);
    }

    [Fact]
    public void Delete_ShouldRemoveDocumentFromIndex_WhenDocumentExists()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 1 },
                new NumberField { FieldName = "field2", Value = 3 }
            }
        };
        _sut.Add(templateName, document1, 1);

        var document2 = new Document
        {
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 2 },
                new NumberField { FieldName = "field2", Value = 4 }
            }
        };
        _sut.Add(templateName, document2, 2);

        // Act
        _sut.Delete(templateName, 1);

        // Assert
        var intervalTree = _sut.GetIntervalTree(templateName, "field1");
        intervalTree.Should().NotBeNull();
        var values = intervalTree!.GetAllValues();
        values.Should().HaveCount(1);
        values.Should().Contain(x => x.DocumentId == 2);
    }

    [Fact]
    public void Delete_ShouldNotRemoveDocumentFromIndex_WhenDocumentDoesNotExist()
    {
        // Arrange
        const string templateName = "template";
        var document1 = new Document
        {
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 1 },
                new NumberField { FieldName = "field2", Value = 3 }
            }
        };
        _sut.Add(templateName, document1, 1);

        var document2 = new Document
        {
            NumberFields = new []
            {
                new NumberField { FieldName = "field1", Value = 2 },
                new NumberField { FieldName = "field2", Value = 4 }
            }
        };
        _sut.Add(templateName, document2, 2);

        // Act
        _sut.Delete(templateName, 3);

        // Assert
        var intervalTree = _sut.GetIntervalTree(templateName, "field1");
        intervalTree.Should().NotBeNull();
        var values = intervalTree!.GetAllValues();
        values.Should().HaveCount(2);
        values.Should().Contain(x => x.DocumentId == 1);
        values.Should().Contain(x => x.DocumentId == 2);
    }
}
