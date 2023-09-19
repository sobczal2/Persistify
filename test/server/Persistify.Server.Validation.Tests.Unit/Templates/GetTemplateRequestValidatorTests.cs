using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Persistify.Requests.Templates;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Templates;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Templates;

public class GetTemplateRequestValidatorTests
{
    private readonly GetTemplateRequestValidator _sut;
    private readonly ITemplateManager _templateManager;

    public GetTemplateRequestValidatorTests()
    {
        _templateManager = Substitute.For<ITemplateManager>();

        _sut = new GetTemplateRequestValidator(_templateManager);
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
        exception.PropertyName.Should().Be("GetTemplateRequest");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new GetTemplateRequest { TemplateName = null! };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("GetTemplateRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var request = new GetTemplateRequest { TemplateName = string.Empty };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("GetTemplateRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var request = new GetTemplateRequest { TemplateName = new string('a', 65) };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value too long");
        exception.PropertyName.Should().Be("GetTemplateRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenTemplateDoesNotExist_ReturnsValidationException()
    {
        // Arrange
        var request = new GetTemplateRequest { TemplateName = "Test" };
        _templateManager.Exists(request.TemplateName).Returns(false);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Template not found");
        exception.PropertyName.Should().Be("GetTemplateRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var request = new GetTemplateRequest { TemplateName = "Test" };
        _templateManager.Exists(request.TemplateName).Returns(true);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
