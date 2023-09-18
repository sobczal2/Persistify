using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Persistify.Requests.Documents;
using Persistify.Requests.Search;
using Persistify.Requests.Shared;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Documents;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Documents;

public class SearchDocumentsRequestValidatorTests
{
    private readonly IValidator<Pagination> _paginationValidator;
    private readonly IValidator<SearchNode> _searchNodeValidator;
    private readonly SearchDocumentsRequestValidator _sut;

    public SearchDocumentsRequestValidatorTests()
    {
        _paginationValidator = Substitute.For<IValidator<Pagination>>();
        _searchNodeValidator = Substitute.For<IValidator<SearchNode>>();

        _sut = new SearchDocumentsRequestValidator(_paginationValidator, _searchNodeValidator);
    }

    [Fact]
    public void Ctor_WhenPaginationValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        var act = () =>
        {
            var unused = new SearchDocumentsRequestValidator(null!, _searchNodeValidator);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenSearchNodeValidatorIsNull_ThrowsArgumentNullException()
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
        _searchNodeValidator.Received().PropertyName = _sut.PropertyName;
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
        exception.PropertyName.Should().Be("SearchDocumentsRequest");
    }

    [Fact]
    public void Validate_WhenTemplateNameIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new SearchDocumentsRequest { TemplateName = null! };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("SearchDocumentsRequest.TemplateName");
    }

    [Fact]
    public void Validate_WhenTemplateNameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var request = new SearchDocumentsRequest { TemplateName = string.Empty };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("SearchDocumentsRequest.TemplateName");
    }

    [Fact]
    public void Validate_WhenPaginationIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new SearchDocumentsRequest { TemplateName = "Test" };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("SearchDocumentsRequest.Pagination");
    }

    [Fact]
    public void Validate_WhenCorrect_CallsPaginationValidatorWithCorrectPropertyName()
    {
        // Arrange
        var request = new SearchDocumentsRequest { TemplateName = "Test", Pagination = new Pagination() };

        List<string> propertyNameAtCall = null!;
        _paginationValidator
            .When(x => x.Validate(Arg.Any<Pagination>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        _sut.Validate(request);

        // Assert
        propertyNameAtCall.Should()
            .BeEquivalentTo(new List<string>(new[] { "SearchDocumentsRequest", "Pagination" }));
    }

    [Fact]
    public void Validate_WhenPaginationValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new SearchDocumentsRequest { TemplateName = "Test", Pagination = new Pagination() };

        var validationException = new ValidationException("Test", "Test");
        _paginationValidator.Validate(Arg.Any<Pagination>()).Returns(validationException);

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().Be(validationException);
    }

    [Fact]
    public void Validate_WhenSearchNodeIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new SearchDocumentsRequest { TemplateName = "Test", Pagination = new Pagination() };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("SearchDocumentsRequest.SearchNode");
    }

    [Fact]
    public void Validate_WhenCorrect_CallsSearchNodeValidatorWithCorrectPropertyName()
    {
        // Arrange
        var request = new SearchDocumentsRequest
        {
            TemplateName = "Test", Pagination = new Pagination(), SearchNode = new SearchNode()
        };

        List<string> propertyNameAtCall = null!;
        _searchNodeValidator
            .When(x => x.Validate(Arg.Any<SearchNode>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        _sut.Validate(request);

        // Assert
        propertyNameAtCall.Should()
            .BeEquivalentTo(new List<string>(new[] { "SearchDocumentsRequest", "SearchNode" }));
    }

    [Fact]
    public void Validate_WhenSearchNodeValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new SearchDocumentsRequest
        {
            TemplateName = "Test", Pagination = new Pagination(), SearchNode = new SearchNode()
        };

        var validationException = new ValidationException("Test", "Test");
        _searchNodeValidator.Validate(Arg.Any<SearchNode>()).Returns(validationException);

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().Be(validationException);
    }

    [Fact]
    public void Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var request = new SearchDocumentsRequest
        {
            TemplateName = "Test", Pagination = new Pagination(), SearchNode = new SearchNode()
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
