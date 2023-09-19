using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Persistify.Requests.Users;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Validation.Users;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Users;

public class RefreshTokenRequestValidatorTests
{
    private readonly IUserManager _userManager;
    private readonly IOptions<TokenSettings> _tokenOptions;

    private RefreshTokenRequestValidator _sut;

    public RefreshTokenRequestValidatorTests()
    {
        _userManager = Substitute.For<IUserManager>();
        _tokenOptions = Substitute.For<IOptions<TokenSettings>>();
        _tokenOptions.Value.Returns(new TokenSettings());

        _sut = new RefreshTokenRequestValidator(_tokenOptions, _userManager);
    }

    [Fact]
    public void Ctor_WhenTokenOptionsIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => _sut = new RefreshTokenRequestValidator(null!, _userManager);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenTokenSettingsIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => _sut = new RefreshTokenRequestValidator(Substitute.For<IOptions<TokenSettings>>(), _userManager);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenUserManagerIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => _sut = new RefreshTokenRequestValidator(_tokenOptions, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyNamesCorrectly()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "RefreshTokenRequest" });
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
        exception.PropertyName.Should().Be("RefreshTokenRequest");
    }

    [Fact]
    public async Task Validate_WhenUsernameIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            Username = null!,
            RefreshToken = "refreshToken"
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("RefreshTokenRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenUsernameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            Username = string.Empty,
            RefreshToken = "refreshToken"
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("RefreshTokenRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenUsernameIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            Username = new string('a', 65),
            RefreshToken = "refreshToken"
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value too long");
        exception.PropertyName.Should().Be("RefreshTokenRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenUserDoesNotExist_ReturnsValidationException()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            Username = "username",
            RefreshToken = "refreshToken"
        };

        _userManager.Exists(request.Username).Returns(false);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("User not found");
        exception.PropertyName.Should().Be("RefreshTokenRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenRefreshTokenIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            Username = "username",
            RefreshToken = null!
        };

        _userManager.Exists(request.Username).Returns(true);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("RefreshTokenRequest.RefreshToken");
    }

    [Fact]
    public async Task Validate_WhenRefreshTokenIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            Username = "username",
            RefreshToken = string.Empty
        };

        _userManager.Exists(request.Username).Returns(true);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("RefreshTokenRequest.RefreshToken");
    }

    [Fact]
    public async Task Validate_WhenRefreshTokensLengthIsInvalid_ReturnsValidationException()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            Username = "username",
            RefreshToken = new string('a', 129)
        };

        _userManager.Exists(request.Username).Returns(true);
        _tokenOptions.Value.Returns(new TokenSettings { RefreshTokenLength = 128 });

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Invalid refresh token");
        exception.PropertyName.Should().Be("RefreshTokenRequest.RefreshToken");
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            Username = "username",
            RefreshToken = new string('a', 128)
        };

        _userManager.Exists(request.Username).Returns(true);
        _tokenOptions.Value.Returns(new TokenSettings { RefreshTokenLength = 128 });

        _sut = new RefreshTokenRequestValidator(_tokenOptions, _userManager);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
