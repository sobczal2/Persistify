using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Helpers.Results;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Validation.Common;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Common;

public class ValidatorTests
{
    [Fact]
    public async Task Validate_WithNullValue_ReturnsValidationException()
    {
        // Arrange
        var validator = new TestValidator();

        // Act
        var result = await validator.ValidateAsync(null!);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be(string.Empty);
    }

    [Fact]
    public async Task Validate_WithNotNullValue_ReturnsResultOk()
    {
        // Arrange
        var validator = new TestValidator { Success = true };

        // Act
        var result = await validator.ValidateAsync("Test");

        // Assert
        result.Failure.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithNotNullValue_ReturnsValidationException()
    {
        // Arrange
        var validator = new TestValidator { Success = false };

        // Act
        var result = await validator.ValidateAsync("Test");

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Test");
        exception.PropertyName.Should().Be(string.Empty);
    }

    private class TestValidator : Validator<string>
    {
        public bool Success { get; set; }

        public override ValueTask<Result> ValidateNotNullAsync(string value)
        {
            if (Success)
            {
                return ValueTask.FromResult(Result.Ok);
            }

            return ValueTask.FromResult<Result>(ValidationException("Test"));
        }
    }
}
