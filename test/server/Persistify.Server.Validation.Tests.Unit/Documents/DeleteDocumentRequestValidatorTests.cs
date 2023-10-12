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

public class DeleteDocumentRequestValidatorTests
{
    private readonly DeleteDocumentRequestValidator _sut;
    private readonly ITemplateManager _templateManager;

    public DeleteDocumentRequestValidatorTests()
    {
        _templateManager = Substitute.For<ITemplateManager>();

        _sut = new DeleteDocumentRequestValidator(_templateManager);
    }

    [Fact]
    public void Ctor_WhenTemplateManagerIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => new DeleteDocumentRequestValidator(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
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
    public async Task Validate_WhenValueIsNull_ReturnsValidationException()
    {
        // Arrange

        // Act
        var result = await _sut.ValidateAsync(null!);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("DeleteDocumentRequest");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new DeleteDocumentRequest { TemplateName = null!, DocumentId = 1 };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("DeleteDocumentRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var request = new DeleteDocumentRequest { TemplateName = new string('a', 65), DocumentId = 1 };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value too long");
        exception.PropertyName.Should().Be("DeleteDocumentRequest.TemplateName");
    }

    [Fact]
    public void Validate_WhenTemplateDoesNotExist_ReturnsValidationException()
    {
        // Arrange
        var request = new DeleteDocumentRequest { TemplateName = "Test", DocumentId = 1 };
        _templateManager.Exists(request.TemplateName).Returns(false);

        // Act
        var result = _sut.ValidateAsync(request).Result;

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Template not found");
        exception.PropertyName.Should().Be("DeleteDocumentRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var request = new DeleteDocumentRequest { TemplateName = string.Empty, DocumentId = 1 };
        _templateManager.Exists(request.TemplateName).Returns(true);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("DeleteDocumentRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenDocumentIdIsZero_ReturnsValidationException()
    {
        // Arrange
        var request = new DeleteDocumentRequest { TemplateName = "Test", DocumentId = 0 };
        _templateManager.Exists(request.TemplateName).Returns(true);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Invalid document id");
        exception.PropertyName.Should().Be("DeleteDocumentRequest.DocumentId");
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var request = new DeleteDocumentRequest { TemplateName = "Test", DocumentId = 1 };
        _templateManager.Exists(request.TemplateName).Returns(true);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
