﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Persistify.Dtos.Common;
using Persistify.Requests.Templates;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Requests.Templates;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Templates;

public class ListTemplatesRequestValidatorTests
{
    private readonly IValidator<PaginationDto> _paginationValidator;
    private readonly ListTemplatesRequestValidator _sut;

    public ListTemplatesRequestValidatorTests()
    {
        _paginationValidator = Substitute.For<IValidator<PaginationDto>>();

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
        var request = new ListTemplatesRequest { PaginationDto = new PaginationDto() };

        List<string> propertyNameAtCall = null!;
        _paginationValidator
            .When(x => x.ValidateAsync(Arg.Any<PaginationDto>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        await _sut.ValidateAsync(request);

        // Assert
        propertyNameAtCall
            .Should()
            .BeEquivalentTo(new List<string>(new[] { "ListTemplatesRequest", "PaginationDto" }));
    }

    [Fact]
    public async Task Validate_WhenPaginationValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new ListTemplatesRequest { PaginationDto = new PaginationDto() };

        var validationException = new StaticValidationPersistifyException("Test", "Test");
        _paginationValidator.ValidateAsync(Arg.Any<PaginationDto>()).Returns(validationException);

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
        var request = new ListTemplatesRequest { PaginationDto = new PaginationDto() };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
