using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Files;
using Persistify.Server.Persistence.Streams;
using Xunit;

namespace Persistify.Server.Management.Tests.Unit.Files;

public class IdleTimeoutFileStreamFactoryTests
{
    private readonly IdleTimeoutFileStreamFactory _sut;

    public IdleTimeoutFileStreamFactoryTests()
    {
        var logger = Substitute.For<ILogger<IdleTimeoutFileStreamFactory>>();
        var storageSettingsOptions = Substitute.For<IOptions<StorageSettings>>();
        storageSettingsOptions.Value.Returns(
            new StorageSettings { DataPath = "dataPath", IdleFileTimeout = TimeSpan.FromMinutes(1) }
        );
        _sut = new IdleTimeoutFileStreamFactory(storageSettingsOptions, logger);
    }

    [Fact]
    public void Ctor_WhenLoggerIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        ILogger<IdleTimeoutFileStreamFactory> logger = null!;
        var storageSettingsOptions = Substitute.For<IOptions<StorageSettings>>();

        // Act
        var action = new Action(() =>
        {
            var unused = new IdleTimeoutFileStreamFactory(storageSettingsOptions, logger);
        });

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenStorageSettingsOptionsIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        IOptions<StorageSettings> storageSettingsOptions = null!;
        var logger = Substitute.For<ILogger<IdleTimeoutFileStreamFactory>>();

        // Act
        var action = new Action(() =>
        {
            var unused = new IdleTimeoutFileStreamFactory(storageSettingsOptions, logger);
        });

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreateStream_WhenRelativePathIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        string relativePath = null!;

        // Act
        var action = new Action(() =>
        {
            var unused = _sut.CreateStream(relativePath);
        });

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreateStream_WhenRelativePathIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var relativePath = string.Empty;

        // Act
        var action = new Action(() =>
        {
            var unused = _sut.CreateStream(relativePath);
        });

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateStream_WhenRelativePathIsValid_ReturnsIdleTimeoutFileStream()
    {
        // Arrange
        var relativePath = "relativePath";

        // Act
        var result = _sut.CreateStream(relativePath);

        // Assert
        result.Should().BeOfType<IdleTimeoutFileStream>();
    }
}
