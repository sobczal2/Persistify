using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistify.Persistance.Exceptions;
using Persistify.Serialization;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Persistance.Template;

public class FileSystemTemplateStorage : ITemplateStorage
{
    private const string DirectoryName = "Templates";
    private const string TemplateIdsFileName = "templateIds.json";
    private readonly string _fullPath;
    private readonly ILogger<FileSystemTemplateStorage> _logger;
    private readonly ISerializer _serializer;
    private readonly string _templateIdsFilePath;


    public FileSystemTemplateStorage(
        ISerializer serializer,
        IOptions<StorageSettings> storageSettings,
        ILogger<FileSystemTemplateStorage> logger
    )
    {
        _serializer = serializer;
        _logger = logger;
        _fullPath = Path.Combine(storageSettings.Value.DataPath, DirectoryName);
        _templateIdsFilePath = Path.Combine(storageSettings.Value.DataPath, TemplateIdsFileName);

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

    public async ValueTask<ConcurrentDictionary<string, long>> GetTemplateIdsAsync()
    {
        if (!File.Exists(_templateIdsFilePath))
        {
            return new ConcurrentDictionary<string, long>();
        }

        await using var fileStream = File.OpenRead(_templateIdsFilePath);
        return await JsonSerializer.DeserializeAsync<ConcurrentDictionary<string, long>>(fileStream)
               ?? throw new PersistenceException("Could not deserialize template ids");
    }

    public async ValueTask SaveTemplateIdsAsync(ConcurrentDictionary<string, long> templateIds)
    {
        await using var fileStream = File.Create(_templateIdsFilePath);
        await JsonSerializer.SerializeAsync(fileStream, templateIds);
    }

    private string GetFilePath(string templateName)
    {
        return Path.Combine(_fullPath, templateName);
    }
}
