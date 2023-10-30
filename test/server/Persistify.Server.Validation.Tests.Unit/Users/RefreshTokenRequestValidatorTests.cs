using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Persistify.Requests.Users;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Validation.Requests.Users;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Users;

public class RefreshTokenRequestValidatorTests
{
    private readonly IOptions<TokenSettings> _tokenOptions;

    private RefreshTokenRequestValidator _sut;

    public RefreshTokenRequestValidatorTests()
    {
        _tokenOptions = Substitute.For<IOptions<TokenSettings>>();
        _tokenOptions.Value.Returns(new TokenSettings());

        _sut = new RefreshTokenRequestValidator(_tokenOptions);
    }

    [Fact]
    public void Ctor_WhenTokenOptionsIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => _sut = new RefreshTokenRequestValidator(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenTokenSettingsIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () =>
            _sut = new RefreshTokenRequestValidator(Substitute.For<IOptions<TokenSettings>>());

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
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("RefreshTokenRequest");
    }

    [Fact]
    public async Task Validate_WhenUsernameIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new RefreshTokenRequest { Username = null!, RefreshToken = "refreshToken" };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
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
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
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
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value too long");
        exception.PropertyName.Should().Be("RefreshTokenRequest.Username");
    }

    [Fact]
    public async Task Validate_WhenRefreshTokenIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new RefreshTokenRequest { Username = "username", RefreshToken = null! };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
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

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("RefreshTokenRequest.RefreshToken");
    }

    [Fact]
    public async Task Validate_WhenRefreshTokensLengthIsInvalid_ReturnsValidationException()
    {
        // Arrange
        var bytes = new byte[129];
        Random.Shared.NextBytes(bytes);
        var refreshToken = Convert.ToBase64String(bytes);
        var request = new RefreshTokenRequest
        {
            Username = "username",
            RefreshToken = refreshToken
        };

        _tokenOptions.Value.Returns(new TokenSettings { RefreshTokenLength = 128 });

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Invalid refresh token");
        exception.PropertyName.Should().Be("RefreshTokenRequest.RefreshToken");
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var bytes = new byte[128];
        Random.Shared.NextBytes(bytes);
        var refreshToken = Convert.ToBase64String(bytes);
        var request = new RefreshTokenRequest
        {
            Username = "username",
            RefreshToken = refreshToken
        };

        _tokenOptions.Value.Returns(new TokenSettings { RefreshTokenLength = 128 });

        _sut = new RefreshTokenRequestValidator(_tokenOptions);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
