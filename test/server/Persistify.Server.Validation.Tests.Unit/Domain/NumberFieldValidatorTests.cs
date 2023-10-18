﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Domain.Templates;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Validation.Dtos.Fields;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Domain;

public class NumberFieldValidatorTests
{
    private readonly NumberFieldDtoValidator _sut;

    public NumberFieldValidatorTests()
    {
        _sut = new NumberFieldDtoValidator();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyName()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "NumberField" });
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
        exception.PropertyName.Should().Be("NumberField");
    }

    [Fact]
    public async Task Validate_WhenNameIsNull_ReturnsValidationException()
    {
        // Arrange
        var value = new NumberField { Name = null! };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("NumberField.Name");
    }

    [Fact]
    public async Task Validate_WhenNameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var value = new NumberField { Name = string.Empty };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("NumberField.Name");
    }

    [Fact]
    public async Task Validate_WhenNameIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var value = new NumberField { Name = new string('a', 65) };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value too long");
        exception.PropertyName.Should().Be("NumberField.Name");
    }

    [Fact]
    public async Task Validate_WhenNameIsCorrect_ReturnsOk()
    {
        // Arrange
        var value = new NumberField { Name = "Name" };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
