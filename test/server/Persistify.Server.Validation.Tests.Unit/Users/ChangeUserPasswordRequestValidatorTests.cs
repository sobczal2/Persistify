﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Persistify.Requests.Users;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Validation.Requests.Users;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Users;

public class ChangeUserPasswordRequestValidatorTests
{
    private readonly IUserManager _userManager;
    private ChangeUserPasswordRequestValidator _sut;

    public ChangeUserPasswordRequestValidatorTests()
    {
        _userManager = Substitute.For<IUserManager>();

        _sut = new ChangeUserPasswordRequestValidator(_userManager);
    }

    [Fact]
    public void Ctor_WhenUserManagerIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => _sut = new ChangeUserPasswordRequestValidator(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyNamesCorrectly()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName
            .Should()
            .BeEquivalentTo(new List<string> { "ChangeUserPasswordRequest" });
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
        exception.PropertyName.Should().Be("ChangeUserPasswordRequest");
    }

    [Fact]
    public async Task Validate_WhenUsernameIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new ChangeUserPasswordRequest { Username = null!, Password = "password" };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("ChangeUserPasswordRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenUsernameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var request = new ChangeUserPasswordRequest { Username = string.Empty, Password = "password" };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("ChangeUserPasswordRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenUsernameIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var request = new ChangeUserPasswordRequest { Username = new string('a', 65), Password = "password" };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value too long");
        exception.PropertyName.Should().Be("ChangeUserPasswordRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenUserDoesNotExist_ReturnsValidationException()
    {
        // Arrange
        var request = new ChangeUserPasswordRequest { Username = "username", Password = "password" };

        _userManager.Exists(request.Username).Returns(false);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("User not found");
        exception.PropertyName.Should().Be("ChangeUserPasswordRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenPasswordIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new ChangeUserPasswordRequest { Username = "username", Password = null! };

        _userManager.Exists(request.Username).Returns(true);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("ChangeUserPasswordRequest.Password");
    }

    [Fact]
    public async Task Validate_WhenPasswordIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var request = new ChangeUserPasswordRequest { Username = "username", Password = string.Empty };

        _userManager.Exists(request.Username).Returns(true);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("ChangeUserPasswordRequest.Password");
    }

    [Fact]
    public async Task Validate_WhenPasswordIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var request = new ChangeUserPasswordRequest { Username = "username", Password = new string('a', 1025) };

        _userManager.Exists(request.Username).Returns(true);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value too long");
        exception.PropertyName.Should().Be("ChangeUserPasswordRequest.Password");
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var request = new ChangeUserPasswordRequest { Username = "username", Password = "password" };

        _userManager.Exists(request.Username).Returns(true);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
