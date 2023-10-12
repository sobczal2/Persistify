using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Domain.Templates;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Validation.Domain;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Domain;

public class BoolFieldValidatorTests
{
    private readonly BoolFieldValidator _sut;

    public BoolFieldValidatorTests()
    {
        _sut = new BoolFieldValidator();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyName()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "BoolField" });
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
        exception.PropertyName.Should().Be("BoolField");
    }

    [Fact]
    public async Task Validate_WhenNameIsNull_ReturnsValidationException()
    {
        // Arrange
        var value = new BoolField { Name = null! };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("BoolField.Name");
    }

    [Fact]
    public async Task Validate_WhenNameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var value = new BoolField { Name = string.Empty };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("BoolField.Name");
    }

    [Fact]
    public async Task Validate_WhenNameIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var value = new BoolField { Name = new string('a', 65) };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value too long");
        exception.PropertyName.Should().Be("BoolField.Name");
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var value = new BoolField { Name = "Test" };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
