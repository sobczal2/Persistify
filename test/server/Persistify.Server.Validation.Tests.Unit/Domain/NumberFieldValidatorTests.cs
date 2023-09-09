using System.Collections.Generic;
using FluentAssertions;
using Persistify.Domain.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Domain;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Domain;

public class NumberFieldValidatorTests
{
    private readonly NumberFieldValidator _sut;

    public NumberFieldValidatorTests()
    {
        _sut = new NumberFieldValidator();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyName()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "NumberField" });
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
        exception.PropertyName.Should().Be("NumberField");
    }

    [Fact]
    public void Validate_WhenNameIsNull_ReturnsValidationException()
    {
        // Arrange
        var value = new NumberField { Name = null! };

        // Act
        var result = _sut.Validate(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("NumberField.Name");
    }

    [Fact]
    public void Validate_WhenNameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var value = new NumberField { Name = string.Empty };

        // Act
        var result = _sut.Validate(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("NumberField.Name");
    }

    [Fact]
    public void Validate_WhenNameIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var value = new NumberField { Name = new string('a', 65) };

        // Act
        var result = _sut.Validate(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name too long");
        exception.PropertyName.Should().Be("NumberField.Name");
    }

    [Fact]
    public void Validate_WhenNameIsCorrect_ReturnsOk()
    {
        // Arrange
        var value = new NumberField { Name = "Name" };

        // Act
        var result = _sut.Validate(value);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
