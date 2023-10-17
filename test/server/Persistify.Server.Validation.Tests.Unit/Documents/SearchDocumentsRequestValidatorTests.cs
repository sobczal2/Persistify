using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Persistify.Domain.Search.Queries.Bool;
using Persistify.Domain.Templates;
using Persistify.Requests.Common;
using Persistify.Requests.Documents;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Documents;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Documents;

public class SearchDocumentsRequestValidatorTests
{
    private readonly IValidator<Pagination> _paginationValidator;
    private readonly SearchDocumentsRequestValidator _sut;
    private readonly ITemplateManager _templateManager;

    public SearchDocumentsRequestValidatorTests()
    {
        _paginationValidator = Substitute.For<IValidator<Pagination>>();
        _templateManager = Substitute.For<ITemplateManager>();

        _sut = new SearchDocumentsRequestValidator(_paginationValidator, _templateManager);
    }

    [Fact]
    public void Ctor_WhenPaginationValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        var act = () =>
        {
            var unused = new SearchDocumentsRequestValidator(null!, _templateManager);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenTemplateManagerIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        var act = () =>
        {
            var unused = new SearchDocumentsRequestValidator(_paginationValidator, null!);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenAllParametersAreNotNull_SetsPropertyNamesCorrectly()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string>(new[] { "SearchDocumentsRequest" }));
        _paginationValidator.Received().PropertyName = _sut.PropertyName;
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
        exception.PropertyName.Should().Be("SearchDocumentsRequest");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new SearchDocumentsRequest { TemplateName = null! };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("SearchDocumentsRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var request = new SearchDocumentsRequest { TemplateName = string.Empty };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("SearchDocumentsRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var request = new SearchDocumentsRequest { TemplateName = new string('a', 65) };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value too long");
        exception.PropertyName.Should().Be("SearchDocumentsRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenTemplateDoesNotExist_ReturnsValidationException()
    {
        // Arrange
        var request = new SearchDocumentsRequest { TemplateName = "Test" };
        _templateManager.Exists(request.TemplateName).Returns(false);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Template not found");
        exception.PropertyName.Should().Be("SearchDocumentsRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenCorrect_CallsPaginationValidatorWithCorrectPropertyName()
    {
        // Arrange
        var request = new SearchDocumentsRequest { TemplateName = "Test", Pagination = new Pagination() };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());

        List<string> propertyNameAtCall = null!;
        _paginationValidator
            .When(x => x.ValidateAsync(Arg.Any<Pagination>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        await _sut.ValidateAsync(request);

        // Assert
        propertyNameAtCall.Should()
            .BeEquivalentTo(new List<string>(new[] { "SearchDocumentsRequest", "Pagination" }));
    }

    [Fact]
    public async Task Validate_WhenPaginationValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new SearchDocumentsRequest { TemplateName = "Test", Pagination = new Pagination() };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());

        var validationException = new StaticValidationPersistifyException("Test", "Test");
        _paginationValidator.ValidateAsync(Arg.Any<Pagination>()).Returns(validationException);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().Be(validationException);
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var request = new SearchDocumentsRequest
        {
            TemplateName = "Test",
            Pagination = new Pagination(),
            SearchQuery = new ExactBoolSearchQuery { FieldName = "Test", Value = true, Boost = 1 }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template
        {
            BoolFields = new List<BoolField> { new() { Name = "Test" } }
        });

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
