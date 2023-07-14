using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistify.Serialization;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Persistance.Document;

public class FileSystemDocumentStorage : IDocumentStorage
{
    private const string DirectoryName = "Documents";
    private readonly string _fullPath;
    private readonly ILogger<FileSystemDocumentStorage> _logger;
    private readonly ISerializer _serializer;

    public FileSystemDocumentStorage(
        ISerializer serializer,
        IOptions<StorageSettings> storageSettings,
        ILogger<FileSystemDocumentStorage> logger
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

    public ValueTask AddAsync(string templateName, long documentId, Protos.Documents.Shared.Document document)
    {
        using var fileStream = File.Create(GetFilePath(templateName, documentId));
        _serializer.Serialize(fileStream, document);

        return ValueTask.CompletedTask;
    }


    public ValueTask<Protos.Documents.Shared.Document?> GetAsync(string templateName, long documentId)
    {
        if (!File.Exists(GetFilePath(templateName, documentId)))
        {
            return ValueTask.FromResult<Protos.Documents.Shared.Document?>(null);
        }

        using var fileStream = File.OpenRead(GetFilePath(templateName, documentId));
        return ValueTask.FromResult<Protos.Documents.Shared.Document?>(
            _serializer.Deserialize<Protos.Documents.Shared.Document>(fileStream));
    }

    public ValueTask<IEnumerable<Protos.Documents.Shared.Document>> GetAllAsync(string templateName)
    {
        var files = Directory.GetFiles(GetDirectoryPath(templateName));

        var documents = new Protos.Documents.Shared.Document[files.Length];

        for (var i = 0; i < files.Length; i++)
        {
            using var fileStream = File.OpenRead(files[i]);
            documents[i] = _serializer.Deserialize<Protos.Documents.Shared.Document>(fileStream);
        }

        return ValueTask.FromResult<IEnumerable<Protos.Documents.Shared.Document>>(documents);
    }

    public ValueTask<bool> ExistsAsync(string templateName, long documentId)
    {
        return ValueTask.FromResult(File.Exists(GetFilePath(templateName, documentId)));
    }

    public ValueTask DeleteAsync(string templateName, long documentId)
    {
        File.Delete(GetFilePath(templateName, documentId));

        return ValueTask.CompletedTask;
    }

    public ValueTask AddSpaceForTemplateAsync(string templateName)
    {
        var directoryPath = GetDirectoryPath(templateName);
        Directory.CreateDirectory(directoryPath);

        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteSpaceForTemplateAsync(string templateName)
    {
        var directoryPath = GetDirectoryPath(templateName);
        Directory.Delete(directoryPath, true);

        return ValueTask.CompletedTask;
    }

    private string GetFilePath(string templateName, long documentId)
    {
        return Path.Combine(_fullPath, templateName, documentId.ToString("x8"));
    }

    private string GetDirectoryPath(string templateName)
    {
        return Path.Combine(_fullPath, templateName);
    }
}
