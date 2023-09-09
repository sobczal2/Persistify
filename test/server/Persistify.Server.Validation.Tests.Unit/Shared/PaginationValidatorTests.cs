using System.Collections.Generic;
using FluentAssertions;
using Persistify.Requests.Shared;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Shared;

public class PaginationValidatorTests
{
    private readonly PaginationValidator _sut;

    public PaginationValidatorTests()
    {
        _sut = new PaginationValidator();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyName()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "Pagination" });
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
        exception.PropertyName.Should().Be("Pagination");
    }

    [Fact]
    public void Validate_WhenPageNumberLessThanZero_ReturnsValidationException()
    {
        // Arrange
        var value = new Pagination { PageNumber = -1, PageSize = 1 };

        // Act
        var result = _sut.Validate(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Page number less than zero");
        exception.PropertyName.Should().Be("Pagination.PageNumber");
    }

    [Fact]
    public void Validate_WhenPageSizeLessThanOrEqualToZero_ReturnsValidationException()
    {
        // Arrange
        var value = new Pagination { PageNumber = 1, PageSize = 0 };

        // Act
        var result = _sut.Validate(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Page size less than or equal to zero");
        exception.PropertyName.Should().Be("Pagination.PageSize");
    }

    [Fact]
    public void Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var value = new Pagination { PageNumber = 1, PageSize = 1 };

        // Act
        var result = _sut.Validate(value);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
