using System;
using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using NSubstitute;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Persistence.Core.FileSystem;
using Xunit;

namespace Persistify.Server.Persistence.Core.Tests.Unit.FileSystem;

public class FileSystemIntLinearRepositoryManagerTests
{
    private string _tempDirectoryPath;
    private readonly IOptions<StorageSettings> _storageSettings;
    private readonly ISystemClock _systemClock;
    private readonly FileSystemIntLinearRepositoryManager _sut;

    public FileSystemIntLinearRepositoryManagerTests()
    {
        _tempDirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDirectoryPath);
        _storageSettings = Substitute.For<IOptions<StorageSettings>>();
        _storageSettings.Value.Returns(new StorageSettings
        {
            DataPath = _tempDirectoryPath,
            RepositorySectorSize = 100
        });
        _systemClock = Substitute.For<ISystemClock>();
        _sut = new FileSystemIntLinearRepositoryManager(
            _storageSettings,
            _systemClock
        );
    }

    [Fact]
    public void Ctor_StoreSettingsOptionsIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        IOptions<StorageSettings> storageSettings = null!;
        var systemClock = Substitute.For<ISystemClock>();

        // Act
        var action = new Action(() => new FileSystemIntLinearRepositoryManager(
            storageSettings,
            systemClock
        ));

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_StoreSettingsIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        IOptions<StorageSettings> storageSettings = Substitute.For<IOptions<StorageSettings>>();
        storageSettings.Value.Returns((StorageSettings)null!);
        var systemClock = Substitute.For<ISystemClock>();

        // Act
        var action = new Action(() => new FileSystemIntLinearRepositoryManager(
            storageSettings,
            systemClock
        ));

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_SystemClockIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var storageSettings = Substitute.For<IOptions<StorageSettings>>();
        ISystemClock systemClock = null!;

        // Act
        var action = new Action(() => new FileSystemIntLinearRepositoryManager(
            storageSettings,
            systemClock
        ));

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }
}
