using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Persistify.Domain.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Domain;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Domain;

// TODO: Add tests for AnalyzerDescriptorValidator
public class TextFieldValidatorTests
{
    private readonly IValidator<AnalyzerDescriptor> _analyzerDescriptorValidator;
    private readonly TextFieldValidator _sut;

    public TextFieldValidatorTests()
    {
        _analyzerDescriptorValidator = Substitute.For<IValidator<AnalyzerDescriptor>>();

        _sut = new TextFieldValidator(_analyzerDescriptorValidator);
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyName()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "TextField" });
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
        exception.PropertyName.Should().Be("TextField");
    }

    [Fact]
    public void Validate_WhenNameIsNull_ReturnsValidationException()
    {
        // Arrange
        var value = new TextField { Name = null! };

        // Act
        var result = _sut.Validate(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("TextField.Name");
    }

    [Fact]
    public void Validate_WhenNameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var value = new TextField { Name = string.Empty };

        // Act
        var result = _sut.Validate(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("TextField.Name");
    }

    [Fact]
    public void Validate_WhenNameIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var value = new TextField { Name = new string('a', 65) };

        // Act
        var result = _sut.Validate(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name too long");
        exception.PropertyName.Should().Be("TextField.Name");
    }

    [Fact]
    public void Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var value = new TextField { Name = "Name", AnalyzerDescriptor = new AnalyzerDescriptor() };

        // Act
        var result = _sut.Validate(value);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
