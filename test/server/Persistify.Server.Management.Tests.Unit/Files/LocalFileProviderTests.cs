using System;
using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Management.Files;
using Xunit;

namespace Persistify.Server.Management.Tests.Unit.Files;

public class LocalFileProviderTests : IDisposable
{
    private readonly ILogger<LocalFileProvider> _logger;
    private readonly IOptions<StorageSettings> _storageSettings;
    private readonly LocalFileProvider _sut;
    private readonly string _tempDirectoryPath;

    public LocalFileProviderTests()
    {
        _tempDirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirectoryPath);

        _logger = Substitute.For<ILogger<LocalFileProvider>>();

        _storageSettings = Substitute.For<IOptions<StorageSettings>>();
        _storageSettings.Value.Returns(new StorageSettings { DataPath = _tempDirectoryPath });

        _sut = new LocalFileProvider(
            _logger,
            _storageSettings
        );
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectoryPath))
        {
            Directory.Delete(_tempDirectoryPath, true);
        }
    }

    [Fact]
    public void Ctor_WhenLoggerIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        ILogger<LocalFileProvider> logger = null!;

        // Act
        var action = new Action(() =>
        {
            var unused = new LocalFileProvider(
                logger,
                _storageSettings
            );
        });

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenStorageSettingsIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        IOptions<StorageSettings> storageSettings = null!;

        // Act
        var action = new Action(() =>
        {
            var unused = new LocalFileProvider(
                _logger,
                storageSettings
            );
        });

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Exists_WhenFileExists_ReturnsTrue()
    {
        // Arrange
        var relativePath = "test.txt";
        var absolutePath = Path.Combine(_tempDirectoryPath, relativePath);
        File.WriteAllText(absolutePath, "test");

        // Act
        var result = _sut.Exists(relativePath);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Exists_WhenFileDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var relativePath = "test.txt";

        // Act
        var result = _sut.Exists(relativePath);

        // Assert
        result.Should().BeFalse();
        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
    }

    [Fact]
    public void Create_WhenFileDoesNotExist_CreatesFile()
    {
        // Arrange
        var relativePath = "test.txt";
        var absolutePath = Path.Combine(_tempDirectoryPath, relativePath);
        File.Exists(absolutePath).Should().BeFalse();

        // Act
        _sut.Create(relativePath);

        // Assert
        File.Exists(absolutePath).Should().BeTrue();
    }

    [Fact]
    public void Create_WhenFileExists_ThrowsIOException()
    {
        // Arrange
        var relativePath = "test.txt";
        var absolutePath = Path.Combine(_tempDirectoryPath, relativePath);
        File.WriteAllText(absolutePath, "test");

        // Act
        var action = new Action(() => _sut.Create(relativePath));

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Delete_WhenFileExists_DeletesFile()
    {
        // Arrange
        var relativePath = "test.txt";
        var absolutePath = Path.Combine(_tempDirectoryPath, relativePath);
        File.WriteAllText(absolutePath, "test");
        File.Exists(absolutePath).Should().BeTrue();

        // Act
        _sut.Delete(relativePath);

        // Assert
        File.Exists(absolutePath).Should().BeFalse();
    }

    [Fact]
    public void Delete_WhenFileDoesNotExist_ThrowsIOException()
    {
        // Arrange
        var relativePath = "test.txt";
        var absolutePath = Path.Combine(_tempDirectoryPath, relativePath);
        File.Exists(absolutePath).Should().BeFalse();

        // Act
        var action = new Action(() => _sut.Delete(relativePath));

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }
}
