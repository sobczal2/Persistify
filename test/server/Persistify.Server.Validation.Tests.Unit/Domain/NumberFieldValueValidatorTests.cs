using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Domain.Documents;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Validation.Domain;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Domain;

public class NumberFieldValueValidatorTests
{
    private readonly NumberFieldValueValidator _sut;

    public NumberFieldValueValidatorTests()
    {
        _sut = new NumberFieldValueValidator();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyName()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "NumberFieldValue" });
    }

    [Fact]
    public async Task Validate_WhenValueIsNull_ReturnsValidationException()
    {
        // Arrange

        // Act
        var result = await _sut.ValidateAsync(null!);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("NumberFieldValue");
    }

    [Fact]
    public async Task Validate_WhenFieldNameIsNull_ReturnsValidationException()
    {
        // Arrange
        var value = new NumberFieldValue { FieldName = null! };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("NumberFieldValue.FieldName");
    }

    [Fact]
    public async Task Validate_WhenFieldNameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var value = new NumberFieldValue { FieldName = string.Empty };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("NumberFieldValue.FieldName");
    }

    [Fact]
    public async Task Validate_WhenFieldNameIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var value = new NumberFieldValue { FieldName = new string('a', 65) };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Name too long");
        exception.PropertyName.Should().Be("NumberFieldValue.FieldName");
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var value = new NumberFieldValue { FieldName = "Name", Value = 1 };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
