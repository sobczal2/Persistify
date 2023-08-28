using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Persistify.Server.Persistence.Core.Files;
using Persistify.Server.Persistence.Core.Files.Exceptions;
using Xunit;

namespace Persistify.Server.Persistence.Core.Tests.Unit.Files;

public class FileManagerTests
{
    private IFileProvider _fileProvider;
    private ILogger<FileManager> _logger;
    private FileManager _sut;

    public FileManagerTests()
    {
        _fileProvider = Substitute.For<IFileProvider>();
        _logger = Substitute.For<ILogger<FileManager>>();
        _sut = null!;
    }

    private void CreateSut(
        IEnumerable<IRequiredFileGroup> requiredFileGroups,
        IEnumerable<IFileGroupForTemplate> fileGroupsForTemplate
    )
    {
        _sut = new FileManager(
            _logger,
            _fileProvider,
            requiredFileGroups,
            fileGroupsForTemplate
        );
    }

    [Fact]
    public void Ctor_WhenLoggerIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        ILogger<FileManager> logger = null!;
        var requiredFileGroups = Substitute.For<IEnumerable<IRequiredFileGroup>>();
        var fileGroupsForTemplate = Substitute.For<IEnumerable<IFileGroupForTemplate>>();

        // Act
        var action = new Action(() =>
        {
            var unused = new FileManager(
                logger,
                _fileProvider,
                requiredFileGroups,
                fileGroupsForTemplate
            );
        });

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenFileProviderIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        IFileProvider fileProvider = null!;
        var requiredFileGroups = Substitute.For<IEnumerable<IRequiredFileGroup>>();
        var fileGroupsForTemplate = Substitute.For<IEnumerable<IFileGroupForTemplate>>();

        // Act
        var action = new Action(() =>
        {
            var unused = new FileManager(
                _logger,
                fileProvider,
                requiredFileGroups,
                fileGroupsForTemplate
            );
        });

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenRequiredFileDescriptorsIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        IEnumerable<IRequiredFileGroup> requiredFileGroups = null!;
        var fileGroupsForTemplate = Substitute.For<IEnumerable<IFileGroupForTemplate>>();

        // Act
        var action = new Action(() =>
        {
            var unused = new FileManager(
                _logger,
                _fileProvider,
                requiredFileGroups,
                fileGroupsForTemplate
            );
        });

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenFileForTemplateDescriptorsIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var requiredFileGroups = Substitute.For<IEnumerable<IRequiredFileGroup>>();
        IEnumerable<IFileGroupForTemplate> fileGroupsForTemplate = null!;

        // Act
        var action = new Action(() =>
        {
            var unused = new FileManager(
                _logger,
                _fileProvider,
                requiredFileGroups,
                fileGroupsForTemplate
            );
        });

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void EnsureRequiredFilesAsync_WhenFileProviderExistsReturnsFalse_CallsFileProviderCreate()
    {
        // Arrange
        var requiredFileGroup = Substitute.For<IRequiredFileGroup>();
        requiredFileGroup.GetFileNames().Returns(new List<string> { "file1" });
        var requiredFileGroups = new List<IRequiredFileGroup> { requiredFileGroup };
        var fileGroupsForTemplate = Substitute.For<IEnumerable<IFileGroupForTemplate>>();
        _fileProvider.Exists("file1").Returns(false);
        CreateSut(requiredFileGroups, fileGroupsForTemplate);

        // Act
        _sut.EnsureRequiredFilesAsync();

        // Assert
        _fileProvider.Received(1).Create(Arg.Any<string>());
    }

    [Fact]
    public void EnsureRequiredFilesAsync_WhenFileProviderExistsReturnsTrue_DoesNotCallFileProviderCreate()
    {
        // Arrange
        var requiredFileGroup = Substitute.For<IRequiredFileGroup>();
        requiredFileGroup.GetFileNames().Returns(new List<string> { "file1" });
        var requiredFileGroups = new List<IRequiredFileGroup> { requiredFileGroup };
        var fileGroupsForTemplate = Substitute.For<IEnumerable<IFileGroupForTemplate>>();
        _fileProvider.Exists("file1").Returns(true);
        CreateSut(requiredFileGroups, fileGroupsForTemplate);

        // Act
        _sut.EnsureRequiredFilesAsync();

        // Assert
        _fileProvider.DidNotReceive().Create(Arg.Any<string>());
    }

    [Fact]
    public void CreateFilesForTemplateAsync_WhenFileProviderExistsReturnsFalse_CallsFileProviderCreate()
    {
        // Arrange
        var fileGroupForTemplate = Substitute.For<IFileGroupForTemplate>();
        fileGroupForTemplate.GetFileNamesForTemplate(Arg.Any<int>()).Returns(new List<string> { "file1" });
        var fileGroupsForTemplate = new List<IFileGroupForTemplate> { fileGroupForTemplate };
        var requiredFileGroups = Substitute.For<IEnumerable<IRequiredFileGroup>>();
        _fileProvider.Exists("file1").Returns(false);
        CreateSut(requiredFileGroups, fileGroupsForTemplate);

        // Act
        _sut.CreateFilesForTemplateAsync(1);

        // Assert
        _fileProvider.Received(1).Create(Arg.Any<string>());
    }

    [Fact]
    public void CreateFilesForTemplateAsync_WhenFileProviderExistsReturnsTrue_ThrowsFileStructureCorruptedException()
    {
        // Arrange
        var fileGroupForTemplate = Substitute.For<IFileGroupForTemplate>();
        fileGroupForTemplate.GetFileNamesForTemplate(Arg.Any<int>()).Returns(new List<string> { "file1" });
        var fileGroupsForTemplate = new List<IFileGroupForTemplate> { fileGroupForTemplate };
        var requiredFileGroups = Substitute.For<IEnumerable<IRequiredFileGroup>>();
        _fileProvider.Exists("file1").Returns(true);
        CreateSut(requiredFileGroups, fileGroupsForTemplate);

        // Act
        var action = new Action(() => _sut.CreateFilesForTemplateAsync(1));

        // Assert
        action.Should().Throw<FileStructureCorruptedException>();
    }

    [Fact]
    public void DeleteFilesForTemplateAsync_WhenFileProviderExistsReturnsTrue_CallsFileProviderDelete()
    {
        // Arrange
        var fileGroupForTemplate = Substitute.For<IFileGroupForTemplate>();
        fileGroupForTemplate.GetFileNamesForTemplate(Arg.Any<int>()).Returns(new List<string> { "file1" });
        var fileGroupsForTemplate = new List<IFileGroupForTemplate> { fileGroupForTemplate };
        var requiredFileGroups = Substitute.For<IEnumerable<IRequiredFileGroup>>();
        _fileProvider.Exists("file1").Returns(true);
        CreateSut(requiredFileGroups, fileGroupsForTemplate);

        // Act
        _sut.DeleteFilesForTemplateAsync(1);

        // Assert
        _fileProvider.Received(1).Delete(Arg.Any<string>());
    }

    [Fact]
    public void DeleteFilesForTemplateAsync_WhenFileProviderExistsReturnsFalse_TrowsFileStructureCorruptedException()
    {
        // Arrange
        var fileGroupForTemplate = Substitute.For<IFileGroupForTemplate>();
        fileGroupForTemplate.GetFileNamesForTemplate(Arg.Any<int>()).Returns(new List<string> { "file1" });
        var fileGroupsForTemplate = new List<IFileGroupForTemplate> { fileGroupForTemplate };
        var requiredFileGroups = Substitute.For<IEnumerable<IRequiredFileGroup>>();
        _fileProvider.Exists("file1").Returns(false);
        CreateSut(requiredFileGroups, fileGroupsForTemplate);

        // Act
        var action = new Action(() => _sut.DeleteFilesForTemplateAsync(1));

        // Assert
        action.Should().Throw<FileStructureCorruptedException>();
    }
}
