using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Persistify.Requests.Documents;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Documents;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Documents;

public class GetDocumentRequestValidatorTests
{
    private readonly GetDocumentRequestValidator _sut;
    private readonly ITemplateManager _templateManager;

    public GetDocumentRequestValidatorTests()
    {
        _templateManager = Substitute.For<ITemplateManager>();

        _sut = new GetDocumentRequestValidator(_templateManager);
    }

    [Fact]
    public void Ctor_WhenTemplateManagerIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => new GetDocumentRequestValidator(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyName()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "GetDocumentRequest" });
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
        exception.PropertyName.Should().Be("GetDocumentRequest");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new GetDocumentRequest { TemplateName = null!, DocumentId = 1 };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("GetDocumentRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var request = new GetDocumentRequest { TemplateName = string.Empty, DocumentId = 1 };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("GetDocumentRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenDocumentIdIsZero_ReturnsValidationException()
    {
        // Arrange
        var request = new GetDocumentRequest { TemplateName = "Test", DocumentId = 0 };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Invalid document id");
        exception.PropertyName.Should().Be("GetDocumentRequest.DocumentId");
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var request = new GetDocumentRequest { TemplateName = "Test", DocumentId = 1 };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
