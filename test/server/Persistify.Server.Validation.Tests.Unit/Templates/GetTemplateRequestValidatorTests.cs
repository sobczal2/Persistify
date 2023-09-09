using System.Collections.Generic;
using FluentAssertions;
using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Templates;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Templates;

public class GetTemplateRequestValidatorTests
{
    private readonly GetTemplateRequestValidator _sut;

    public GetTemplateRequestValidatorTests()
    {
        _sut = new GetTemplateRequestValidator();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyName()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "GetTemplateRequest" });
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
        exception.PropertyName.Should().Be("GetTemplateRequest");
    }

    [Fact]
    public void Validate_WhenTemplateIdIsZero_ReturnsValidationException()
    {
        // Arrange
        var request = new GetTemplateRequest
        {
            TemplateId = 0
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Invalid template id");
        exception.PropertyName.Should().Be("GetTemplateRequest.TemplateId");
    }

    [Fact]
    public void Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var request = new GetTemplateRequest
        {
            TemplateId = 1
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
