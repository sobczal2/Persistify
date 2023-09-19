using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Requests.Shared;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Validation.Shared;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Shared;

public class EmptyRequestValidatorTests
{
    private readonly EmptyRequestValidator _sut;

    public EmptyRequestValidatorTests()
    {
        _sut = new EmptyRequestValidator();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyName()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "EmptyRequest" });
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
        exception.PropertyName.Should().Be("EmptyRequest");
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var request = new EmptyRequest();

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
