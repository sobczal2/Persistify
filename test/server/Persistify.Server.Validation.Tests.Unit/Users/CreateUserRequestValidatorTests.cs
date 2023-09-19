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

public class CreateUserRequestValidatorTests
{
    private readonly IUserManager _userManager;

    private CreateUserRequestValidator _sut;

    public CreateUserRequestValidatorTests()
    {
        _userManager = Substitute.For<IUserManager>();

        _sut = new CreateUserRequestValidator(_userManager);
    }

    [Fact]
    public void Ctor_WhenUserManagerIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => _sut = new CreateUserRequestValidator(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyNamesCorrectly()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "CreateUserRequest" });
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
        exception.PropertyName.Should().Be("CreateUserRequest");
    }

    [Fact]
    public async Task Validate_WhenUsernameIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Username = null!,
            Password = "password"
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("CreateUserRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenUsernameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Username = string.Empty,
            Password = "password"
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("CreateUserRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenUsernameIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Username = new string('a', 65),
            Password = "password"
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value too long");
        exception.PropertyName.Should().Be("CreateUserRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenUserWithThatNameAlreadyExists_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Username = "username",
            Password = "password"
        };
        _userManager.Exists(request.Username).Returns(true);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("User already exists");
        exception.PropertyName.Should().Be("CreateUserRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenPasswordIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Username = "username",
            Password = null!
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("CreateUserRequest.Password");
    }

    [Fact]
    public async Task Validate_WhenPasswordIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Username = "username",
            Password = string.Empty
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("CreateUserRequest.Password");
    }

    [Fact]
    public async Task Validate_WhenPasswordIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Username = "username",
            Password = new string('a', 1025)
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value too long");
        exception.PropertyName.Should().Be("CreateUserRequest.Password");
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Username = "username",
            Password = "password"
        };
        _userManager.Exists(request.Username).Returns(false);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
