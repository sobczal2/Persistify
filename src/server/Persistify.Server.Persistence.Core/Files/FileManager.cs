using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Persistify.Server.Persistence.Core.Files.Exceptions;

namespace Persistify.Server.Persistence.Core.Files;

public class FileManager : IFileManager
{
    private readonly IEnumerable<IRequiredFileDescriptor> _requiredFileDescriptors;
    private readonly IEnumerable<IFileForTemplateDescriptor> _fileForTemplateDescriptors;
    private readonly ILogger<FileManager> _logger;
    private readonly IFileProvider _fileProvider;

    public FileManager(
        ILogger<FileManager> logger,
        IFileProvider fileProvider,
        IEnumerable<IRequiredFileDescriptor> requiredFileDescriptors,
        IEnumerable<IFileForTemplateDescriptor> fileForTemplateDescriptors
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        _requiredFileDescriptors =
            requiredFileDescriptors ?? throw new ArgumentNullException(nameof(requiredFileDescriptors));
        _fileForTemplateDescriptors = fileForTemplateDescriptors ??
                                      throw new ArgumentNullException(nameof(fileForTemplateDescriptors));
    }

    public void EnsureRequiredFilesAsync()
    {
        foreach (var fileDescriptor in _requiredFileDescriptors)
        {
            _logger.LogInformation("Ensuring required files for {FileDescriptorName}", fileDescriptor.GetType().Name);
            var requiredFiles = fileDescriptor.GetRequiredFilesNames();
            foreach (var requiredFile in requiredFiles)
            {
                _logger.LogInformation("Ensuring required file {RequiredFile}", requiredFile);
                if (_fileProvider.Exists(requiredFile))
                {
                    _logger.LogInformation("Required file {RequiredFile} already exists", requiredFile);
                    continue;
                }

                _logger.LogInformation("Creating required file {RequiredFile}", requiredFile);
                _fileProvider.Create(requiredFile);
            }
        }
    }

    public void CreateFilesForTemplateAsync(int templateId)
    {
        foreach (var fileDescriptor in _fileForTemplateDescriptors)
        {
            _logger.LogInformation("Creating files for template {TemplateId} for {FileDescriptorName}", templateId,
                fileDescriptor.GetType().Name);
            var files = fileDescriptor.GetFilesNamesForTemplate(templateId);
            foreach (var file in files)
            {
                _logger.LogInformation("Creating file {File}", file);
                if (_fileProvider.Exists(file))
                {
                    throw new FileStructureCorruptedException();
                }

                _logger.LogInformation("Creating file {File}", file);
                _fileProvider.Create(file);
            }
        }
    }

    public void DeleteFilesForTemplateAsync(int templateId)
    {
        foreach (var fileDescriptor in _fileForTemplateDescriptors)
        {
            _logger.LogInformation("Deleting files for template {TemplateId} for {FileDescriptorName}", templateId,
                fileDescriptor.GetType().Name);
            var files = fileDescriptor.GetFilesNamesForTemplate(templateId);
            foreach (var file in files)
            {
                _logger.LogInformation("Deleting file {File}", file);
                if (!_fileProvider.Exists(file))
                {
                    throw new FileStructureCorruptedException();
                }

                _logger.LogInformation("Deleting file {File}", file);
                _fileProvider.Delete(file);
            }
        }
    }
}
