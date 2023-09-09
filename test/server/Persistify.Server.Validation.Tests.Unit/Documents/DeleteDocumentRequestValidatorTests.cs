using System.Collections.Generic;
using FluentAssertions;
using Persistify.Requests.Documents;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Documents;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Documents;

public class DeleteDocumentRequestValidatorTests
{
    private readonly DeleteDocumentRequestValidator _sut;

    public DeleteDocumentRequestValidatorTests()
    {
        _sut = new DeleteDocumentRequestValidator();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyName()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "DeleteDocumentRequest" });
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
        exception.PropertyName.Should().Be("DeleteDocumentRequest");
    }

    [Fact]
    public void Validate_WhenTemplateNameIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new DeleteDocumentRequest
        {
            TemplateName = null!,
            DocumentId = 1
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("DeleteDocumentRequest.TemplateName");
    }

    [Fact]
    public void Validate_WhenTemplateNameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var request = new DeleteDocumentRequest
        {
            TemplateName = string.Empty,
            DocumentId = 1
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("DeleteDocumentRequest.TemplateName");
    }

    [Fact]
    public void Validate_WhenDocumentIdIsZero_ReturnsValidationException()
    {
        // Arrange
        var request = new DeleteDocumentRequest
        {
            TemplateName = "Test",
            DocumentId = 0
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Invalid document id");
        exception.PropertyName.Should().Be("DeleteDocumentRequest.DocumentId");
    }

    [Fact]
    public void Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var request = new DeleteDocumentRequest
        {
            TemplateName = "Test",
            DocumentId = 1
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
