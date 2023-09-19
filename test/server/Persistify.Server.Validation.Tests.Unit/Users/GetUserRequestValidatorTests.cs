using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Persistify.Requests.Users;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Validation.Users;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Users;

public class GetUserRequestValidatorTests
{
    private readonly IUserManager _userManager;

    private GetUserRequestValidator _sut;

    public GetUserRequestValidatorTests()
    {
        _userManager = Substitute.For<IUserManager>();

        _sut = new GetUserRequestValidator(_userManager);
    }

    [Fact]
    public void Ctor_WhenUserManagerIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => _sut = new GetUserRequestValidator(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyNamesCorrectly()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "GetUserRequest" });
    }

    [Fact]
    public async Task Validate_WhenValueIsNull_ReturnsValidationException()
    {
        // Arrange

        // Act
        var result = await _sut.ValidateAsync(null!);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("GetUserRequest");
    }

    [Fact]
    public async Task Validate_WhenUsernameIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new GetUserRequest
        {
            Username = null!
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("GetUserRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenUsernameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var request = new GetUserRequest
        {
            Username = string.Empty
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("GetUserRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenUsernameIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var request = new GetUserRequest
        {
            Username = new string('a', 65)
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value too long");
        exception.PropertyName.Should().Be("GetUserRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenUserDoesNotExist_ReturnsValidationException()
    {
        // Arrange
        var request = new GetUserRequest
        {
            Username = "username"
        };
        _userManager.Exists(request.Username).Returns(false);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("User not found");
        exception.PropertyName.Should().Be("GetUserRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var request = new GetUserRequest
        {
            Username = "username"
        };
        _userManager.Exists(request.Username).Returns(true);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
