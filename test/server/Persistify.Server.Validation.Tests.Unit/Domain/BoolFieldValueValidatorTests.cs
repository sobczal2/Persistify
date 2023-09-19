using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Domain.Documents;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Validation.Domain;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Domain;

public class BoolFieldValueValidatorTests
{
    private readonly BoolFieldValueValidator _sut;

    public BoolFieldValueValidatorTests()
    {
        _sut = new BoolFieldValueValidator();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyName()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "BoolFieldValue" });
    }

    [Fact]
    public async Task Validate_WhenValueIsNull_ReturnsValidationException()
    {
        // Arrange

        // Act
        var result = await _sut.ValidateAsync(null!);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("BoolFieldValue");
    }

    [Fact]
    public async Task Validate_WhenFieldNameIsNull_ReturnsValidationException()
    {
        // Arrange
        var value = new BoolFieldValue { FieldName = null! };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("BoolFieldValue.FieldName");
    }

    [Fact]
    public async Task Validate_WhenFieldNameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var value = new BoolFieldValue { FieldName = string.Empty };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("BoolFieldValue.FieldName");
    }

    [Fact]
    public async Task Validate_WhenFieldNameIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var value = new BoolFieldValue { FieldName = new string('a', 65) };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name too long");
        exception.PropertyName.Should().Be("BoolFieldValue.FieldName");
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var value = new BoolFieldValue { FieldName = "Name" };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
