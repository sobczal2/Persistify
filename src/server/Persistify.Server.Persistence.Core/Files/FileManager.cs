using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Persistify.Server.Persistence.Core.Files.Exceptions;

namespace Persistify.Server.Persistence.Core.Files;

public class FileManager : IFileManager
{
    private readonly IEnumerable<IRequiredFileGroup> _requiredFileGroups;
    private readonly IEnumerable<IFileGroupForTemplate> _fileGroupsForTemplate;
    private readonly ILogger<FileManager> _logger;
    private readonly IFileProvider _fileProvider;

    public FileManager(
        ILogger<FileManager> logger,
        IFileProvider fileProvider,
        IEnumerable<IRequiredFileGroup> requiredFileGroups,
        IEnumerable<IFileGroupForTemplate> fileGroupsForTemplate
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        _requiredFileGroups =
            requiredFileGroups ?? throw new ArgumentNullException(nameof(requiredFileGroups));
        _fileGroupsForTemplate = fileGroupsForTemplate ??
                                 throw new ArgumentNullException(nameof(fileGroupsForTemplate));
    }

    public void EnsureRequiredFilesAsync()
    {
        foreach (var fileGroup in _requiredFileGroups)
        {
            _logger.LogTrace("Ensuring required files for {FileGroupName}", fileGroup.FileGroupName);
            var fileNames = fileGroup.GetFileNames();
            foreach (var fileName in fileNames)
            {
                _logger.LogTrace("Ensuring required file {FileName}", fileName);
                if (_fileProvider.Exists(fileName))
                {
                    _logger.LogTrace("Required file {FileName} already exists", fileName);
                    continue;
                }

                _logger.LogTrace("Creating required file {FileName}", fileName);
                _fileProvider.Create(fileName);
            }
        }
    }

    public void CreateFilesForTemplateAsync(int templateId)
    {
        foreach (var fileGroup in _fileGroupsForTemplate)
        {
            _logger.LogTrace("Creating files for template {TemplateId} for {FileGroupName}", templateId,
                fileGroup.FileGroupName);
            var fileNames = fileGroup.GetFileNamesForTemplate(templateId);
            foreach (var fileName in fileNames)
            {
                _logger.LogTrace("Creating file {FileName}", fileName);
                if (_fileProvider.Exists(fileName))
                {
                    throw new FileStructureCorruptedException();
                }

                _logger.LogTrace("Creating file {FileName}", fileName);
                _fileProvider.Create(fileName);
            }
        }
    }

    public void DeleteFilesForTemplateAsync(int templateId)
    {
        foreach (var fileGroup in _fileGroupsForTemplate)
        {
            _logger.LogTrace("Deleting files for template {TemplateId} for {FileGroupName}", templateId,
                fileGroup.FileGroupName);
            var fileNames = fileGroup.GetFileNamesForTemplate(templateId);
            foreach (var fileName in fileNames)
            {
                _logger.LogTrace("Deleting file {FileName}", fileName);
                if (!_fileProvider.Exists(fileName))
                {
                    throw new FileStructureCorruptedException();
                }

                _logger.LogTrace("Deleting file {FileName}", fileName);
                _fileProvider.Delete(fileName);
            }
        }
    }
}
