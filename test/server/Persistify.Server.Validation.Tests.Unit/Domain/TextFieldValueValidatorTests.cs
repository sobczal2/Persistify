using System.Collections.Generic;
using FluentAssertions;
using Persistify.Domain.Documents;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Domain;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Domain;

public class TextFieldValueValidatorTests
{
    private readonly TextFieldValueValidator _sut;

    public TextFieldValueValidatorTests()
    {
        _sut = new TextFieldValueValidator();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyName()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "TextFieldValue" });
    }

    [Fact]
    public void Validate_WhenValueIsNull_ReturnsValidationException()
    {
        // Arrange

        // Act
        var result = _sut.Validate(null!);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("TextFieldValue");
    }

    [Fact]
    public void Validate_WhenFieldNameIsNull_ReturnsValidationException()
    {
        // Arrange
        var value = new TextFieldValue { FieldName = null! };

        // Act
        var result = _sut.Validate(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("TextFieldValue.FieldName");
    }

    [Fact]
    public void Validate_WhenFieldNameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var value = new TextFieldValue { FieldName = string.Empty };

        // Act
        var result = _sut.Validate(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("TextFieldValue.FieldName");
    }

    [Fact]
    public void Validate_WhenFieldNameIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var value = new TextFieldValue { FieldName = new string('a', 65) };

        // Act
        var result = _sut.Validate(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name too long");
        exception.PropertyName.Should().Be("TextFieldValue.FieldName");
    }

    [Fact]
    public void Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var value = new TextFieldValue { FieldName = "Name" };

        // Act
        var result = _sut.Validate(value);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
