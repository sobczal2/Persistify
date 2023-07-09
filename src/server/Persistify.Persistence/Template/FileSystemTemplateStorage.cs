using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistify.Serialization;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Persistance.Template;

public class FileSystemTemplateStorage : ITemplateStorage
{
    private const string DirectoryName = "Templates";
    private readonly string _fullPath;
    private readonly ILogger<FileSystemTemplateStorage> _logger;
    private readonly ISerializer _serializer;


    public FileSystemTemplateStorage(
        ISerializer serializer,
        IOptions<StorageSettings> storageSettings,
        ILogger<FileSystemTemplateStorage> logger
    )
    {
        _serializer = serializer;
        _logger = logger;
        _fullPath = Path.Combine(storageSettings.Value.DataPath, DirectoryName);

        if (Directory.Exists(_fullPath))
        {
            return;
        }

        Directory.CreateDirectory(_fullPath);

        _logger.LogInformation("Created directory {Directory}", _fullPath);
    }

    public ValueTask AddAsync(Protos.Templates.Shared.Template template)
    {
        try
        {
            using var fileStream = File.Create(GetFilePath(template.Name));
            _serializer.Serialize(fileStream, template);
        }
        catch (IOException)
        {
            _logger.LogWarning("Could not write template {TemplateName}", template.Name);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask<Protos.Templates.Shared.Template?> GetAsync(string templateName)
    {
        try
        {
            using var fileStream = File.OpenRead(GetFilePath(templateName));
            return ValueTask.FromResult<Protos.Templates.Shared.Template?>(
                _serializer.Deserialize<Protos.Templates.Shared.Template>(fileStream));
        }
        catch (IOException)
        {
            _logger.LogWarning("Could not read template {TemplateName}", templateName);
        }

        return ValueTask.FromResult<Protos.Templates.Shared.Template?>(null);
    }

    public ValueTask<IEnumerable<Protos.Templates.Shared.Template>> GetAllAsync()
    {
        var files = Directory.GetFiles(_fullPath);
        var templates = new Protos.Templates.Shared.Template[files.Length];

        for (var i = 0; i < files.Length; i++)
        {
            try
            {
                using var fileStream = File.OpenRead(files[i]);
                templates[i] = _serializer.Deserialize<Protos.Templates.Shared.Template>(fileStream);
            }
            catch (IOException)
            {
                _logger.LogWarning("Could not read template {TemplateName}", files[i]);
            }
        }

        return ValueTask.FromResult<IEnumerable<Protos.Templates.Shared.Template>>(templates);
    }

    public ValueTask DeleteAsync(string templateName)
    {
        try
        {
            File.Delete(GetFilePath(templateName));
        }
        catch (IOException)
        {
            _logger.LogWarning("Could not delete template {TemplateName}", templateName);
        }

        return ValueTask.CompletedTask;
    }

    private string GetFilePath(string templateName)
    {
        return Path.Combine(_fullPath, templateName);
    }
}
