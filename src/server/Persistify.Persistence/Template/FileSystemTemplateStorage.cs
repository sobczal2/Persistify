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
            _logger.LogInformation("Found directory {Directory}, skipping creation", _fullPath);
            return;
        }

        Directory.CreateDirectory(_fullPath);

        _logger.LogInformation("Created directory {Directory}", _fullPath);
    }

    public ValueTask AddAsync(Protos.Templates.Shared.Template template)
    {
        using var fileStream = File.Create(GetFilePath(template.Name));
        _serializer.Serialize(fileStream, template);

        return ValueTask.CompletedTask;
    }

    public ValueTask<Protos.Templates.Shared.Template?> GetAsync(string templateName)
    {
        var filePath = GetFilePath(templateName);
        if (!File.Exists(filePath))
        {
            return ValueTask.FromResult<Protos.Templates.Shared.Template?>(null);
        }

        using var fileStream = File.OpenRead(filePath);
        return ValueTask.FromResult<Protos.Templates.Shared.Template?>(
            _serializer.Deserialize<Protos.Templates.Shared.Template>(fileStream));
    }

    public ValueTask<bool> ExistsAsync(string templateName)
    {
        return ValueTask.FromResult(File.Exists(GetFilePath(templateName)));
    }

    public ValueTask<IEnumerable<Protos.Templates.Shared.Template>> GetAllAsync()
    {
        var files = Directory.GetFiles(_fullPath);
        var templates = new Protos.Templates.Shared.Template[files.Length];

        for (var i = 0; i < files.Length; i++)
        {
            using var fileStream = File.OpenRead(files[i]);
            templates[i] = _serializer.Deserialize<Protos.Templates.Shared.Template>(fileStream);
        }

        return ValueTask.FromResult<IEnumerable<Protos.Templates.Shared.Template>>(templates);
    }

    public ValueTask DeleteAsync(string templateName)
    {
        File.Delete(GetFilePath(templateName));

        return ValueTask.CompletedTask;
    }

    private string GetFilePath(string templateName)
    {
        return Path.Combine(_fullPath, templateName);
    }
}
