using FluentAssertions;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Common;

public class ValidatorTests
{
    private class TestValidator : Validator<string>
    {
        public bool Success { get; set; }
        public override Result ValidateNotNull(string value)
        {
            if (Success)
            {
                return Result.Ok;
            }
            else
            {
                return ValidationException("Test");
            }
        }
    }

    [Fact]
    public void Validate_WithNullValue_ReturnsValidationException()
    {
        // Arrange
        var validator = new TestValidator();

        // Act
        var result = validator.Validate(null!);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be(string.Empty);
    }

    [Fact]
    public void Validate_WithNotNullValue_ReturnsResultOk()
    {
        // Arrange
        var validator = new TestValidator { Success = true };

        // Act
        var result = validator.Validate("Test");

        // Assert
        result.Failure.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithNotNullValue_ReturnsValidationException()
    {
        // Arrange
        var validator = new TestValidator { Success = false };

        // Act
        var result = validator.Validate("Test");

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Test");
        exception.PropertyName.Should().Be(string.Empty);
    }
}
