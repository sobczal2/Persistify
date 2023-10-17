using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Persistify.Requests.Common;
using Persistify.Requests.Templates;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Templates;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Templates;

public class ListTemplatesRequestValidatorTests
{
    private readonly IValidator<Pagination> _paginationValidator;
    private readonly ListTemplatesRequestValidator _sut;

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
        exception.PropertyName.Should().Be("ListTemplatesRequest");
    }

    [Fact]
    public async Task Validate_WhenCorrect_CallsPaginationValidatorWithCorrectPropertyName()
    {
        // Arrange
        var request = new ListTemplatesRequest { Pagination = new Pagination() };

        List<string> propertyNameAtCall = null!;
        _paginationValidator
            .When(x => x.ValidateAsync(Arg.Any<Pagination>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        await _sut.ValidateAsync(request);

        // Assert
        propertyNameAtCall.Should()
            .BeEquivalentTo(new List<string>(new[] { "ListTemplatesRequest", "Pagination" }));
    }

    [Fact]
    public async Task Validate_WhenPaginationValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new ListTemplatesRequest { Pagination = new Pagination() };

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
        var request = new ListTemplatesRequest { Pagination = new Pagination() };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
