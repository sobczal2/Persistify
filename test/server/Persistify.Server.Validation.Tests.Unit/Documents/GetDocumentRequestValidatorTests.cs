using System.Collections.Generic;
using FluentAssertions;
using Persistify.Requests.Documents;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Documents;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Documents;

public class GetDocumentRequestValidatorTests
{
    private readonly GetDocumentRequestValidator _sut;

    public GetDocumentRequestValidatorTests()
    {
        _sut = new GetDocumentRequestValidator();
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
        exception.PropertyName.Should().Be("GetDocumentRequest");
    }

    [Fact]
    public void Validate_WhenTemplateNameIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new GetDocumentRequest { TemplateName = null!, DocumentId = 1 };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("GetDocumentRequest.TemplateName");
    }

    [Fact]
    public void Validate_WhenTemplateNameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var request = new GetDocumentRequest { TemplateName = string.Empty, DocumentId = 1 };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("GetDocumentRequest.TemplateName");
    }

    [Fact]
    public void Validate_WhenDocumentIdIsZero_ReturnsValidationException()
    {
        // Arrange
        var request = new GetDocumentRequest { TemplateName = "Test", DocumentId = 0 };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Invalid document id");
        exception.PropertyName.Should().Be("GetDocumentRequest.DocumentId");
    }

    [Fact]
    public void Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var request = new GetDocumentRequest { TemplateName = "Test", DocumentId = 1 };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
