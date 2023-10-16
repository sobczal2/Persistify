using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Requests.Shared;
using Persistify.Server.ErrorHandling.Exceptions;
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
        exception.PropertyName.Should().Be("Pagination");
    }

    [Fact]
    public async Task Validate_WhenPageNumberLessThanZero_ReturnsValidationException()
    {
        // Arrange
        var value = new Pagination { PageNumber = -1, PageSize = 1 };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Page number less than zero");
        exception.PropertyName.Should().Be("Pagination.PageNumber");
    }

    [Fact]
    public async Task Validate_WhenPageSizeLessThanOrEqualToZero_ReturnsValidationException()
    {
        // Arrange
        var value = new Pagination { PageNumber = 1, PageSize = 0 };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Page size less than or equal to zero");
        exception.PropertyName.Should().Be("Pagination.PageSize");
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var value = new Pagination { PageNumber = 1, PageSize = 1 };

        // Act
        var result = await _sut.ValidateAsync(value);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
