using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Persistify.Requests.Shared;
using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Templates;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Templates;

public class ListTemplatesRequestValidatorTests
{
    private readonly ListTemplatesRequestValidator _sut;

    private readonly IValidator<Pagination> _paginationValidator;

    public ListTemplatesRequestValidatorTests()
    {
        _paginationValidator = Substitute.For<IValidator<Pagination>>();

        _sut = new ListTemplatesRequestValidator(_paginationValidator);
    }

    [Fact]
    public void Ctor_WhenPaginationValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => new ListTemplatesRequestValidator(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyNamesCorrectly()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "ListTemplatesRequest" });
        _paginationValidator.Received().PropertyName = _sut.PropertyName;
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
        exception.PropertyName.Should().Be("ListTemplatesRequest");
    }

    [Fact]
    public void Validate_WhenPaginationIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new ListTemplatesRequest { Pagination = null! };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("ListTemplatesRequest.Pagination");
    }

    [Fact]
    public void Validate_WhenCorrect_CallsPaginationValidatorWithCorrectPropertyName()
    {
        // Arrange
        var request = new ListTemplatesRequest
        {
            Pagination = new Pagination()
        };

        List<string> propertyNameAtCall = null!;
        _paginationValidator
            .When(x => x.Validate(Arg.Any<Pagination>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        _sut.Validate(request);

        // Assert
        propertyNameAtCall.Should()
            .BeEquivalentTo(new List<string>(new[] { "ListTemplatesRequest", "Pagination" }));
    }

    [Fact]
    public void Validate_WhenPaginationValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new ListTemplatesRequest
        {
            Pagination = new Pagination()
        };

        var validationException = new ValidationException("Test", "Test");
        _paginationValidator.Validate(Arg.Any<Pagination>()).Returns(validationException);

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
        var request = new ListTemplatesRequest
        {
            Pagination = new Pagination()
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
