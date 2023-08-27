using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Persistify.Server.Persistence.Core.Files;
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
        IEnumerable<IRequiredFileDescriptor> requiredFileDescriptors,
        IEnumerable<IFileForTemplateDescriptor> fileForTemplateDescriptors
    )
    {
        _sut = new FileManager(
            _logger,
            _fileProvider,
            requiredFileDescriptors,
            fileForTemplateDescriptors
        );
    }

    [Fact]
    public void Ctor_WhenLoggerIsNull_ThrowsArgumentNullException()
    {

    }
}
