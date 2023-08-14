using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Persistence.Core.Files.Exceptions;

namespace Persistify.Server.Persistence.Core.Files;

public class FileManager : IFileManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FileManager> _logger;
    private readonly StorageSettings _storageSettings;

    public FileManager(
        IServiceProvider serviceProvider,
        ILogger<FileManager> logger,
        IOptions<StorageSettings> storageSettingsOptions
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _storageSettings = storageSettingsOptions.Value;
    }

    public void EnsureRequiredFilesAsync()
    {
        var fileDescriptors = _serviceProvider.GetServices<IRequiredFileDescriptor>();

        foreach (var fileDescriptor in fileDescriptors)
        {
            _logger.LogInformation("Ensuring required files for {FileDescriptorName}", fileDescriptor.GetType().Name);
            var requiredFiles = fileDescriptor.GetRequiredFilesNames();
            foreach (var requiredFile in requiredFiles)
            {
                _logger.LogInformation("Ensuring required file {RequiredFile}", requiredFile);
                var filePath = Path.Combine(_storageSettings.DataPath, requiredFile);
                if (File.Exists(filePath))
                {
                    _logger.LogInformation("Required file {RequiredFile} already exists", requiredFile);
                    continue;
                }

                _logger.LogInformation("Creating required file {RequiredFile}", requiredFile);
                File.Create(filePath);
            }
        }
    }

    public void CreateFilesForTemplateAsync(int templateId)
    {
        var fileDescriptors = _serviceProvider.GetServices<IFileForTemplateDescriptor>();

        foreach (var fileDescriptor in fileDescriptors)
        {
            _logger.LogInformation("Creating files for template {TemplateId} for {FileDescriptorName}", templateId, fileDescriptor.GetType().Name);
            var files = fileDescriptor.GetFilesNamesForTemplate(templateId);
            foreach (var file in files)
            {
                _logger.LogInformation("Creating file {File}", file);
                var filePath = Path.Combine(_storageSettings.DataPath, file);
                if (File.Exists(filePath))
                {
                    throw new FileStructureCorruptedException();
                }

                _logger.LogInformation("Creating file {File}", file);
                File.Create(filePath);
            }
        }
    }

    public void DeleteFilesForTemplateAsync(int templateId)
    {
        var fileDescriptors = _serviceProvider.GetServices<IFileForTemplateDescriptor>();

        foreach (var fileDescriptor in fileDescriptors)
        {
            _logger.LogInformation("Deleting files for template {TemplateId} for {FileDescriptorName}", templateId, fileDescriptor.GetType().Name);
            var files = fileDescriptor.GetFilesNamesForTemplate(templateId);
            foreach (var file in files)
            {
                _logger.LogInformation("Deleting file {File}", file);
                var filePath = Path.Combine(_storageSettings.DataPath, file);
                if (!File.Exists(filePath))
                {
                    throw new FileStructureCorruptedException();
                }

                _logger.LogInformation("Deleting file {File}", file);
                File.Delete(filePath);
            }
        }
    }
}
