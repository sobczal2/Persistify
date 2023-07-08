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
    private readonly ISerializer _serializer;
    private readonly ILogger<FileSystemDocumentStorage> _logger;
    private const string DirectoryName = "Documents";
    private readonly string _fullPath;

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
            return;
        }

        Directory.CreateDirectory(_fullPath);

        _logger.LogInformation("Created directory {Directory}", _fullPath);
    }

    private string GetFilePath(string templateName, ulong documentId)
    {
        return Path.Combine(_fullPath, templateName, documentId.ToString("x8"));
    }

    private string GetDirectoryPath(string templateName)
    {
        return Path.Combine(_fullPath, templateName);
    }

    public ValueTask AddAsync(string templateName, ulong documentId, Protos.Documents.Shared.Document document)
    {
        try
        {
            using var fileStream = File.Create(GetFilePath(templateName, documentId));
            _serializer.Serialize(fileStream, document);
        }
        catch (IOException)
        {
            _logger.LogWarning("Could not write document {DocumentId} for template {TemplateName}", documentId,
                templateName);
        }

        return ValueTask.CompletedTask;
    }


    public ValueTask<Protos.Documents.Shared.Document?> GetAsync(string templateName, ulong documentId)
    {
        try
        {
            using var fileStream = File.OpenRead(GetFilePath(templateName, documentId));
            return ValueTask.FromResult(_serializer.Deserialize<Protos.Documents.Shared.Document>(fileStream))!;
        }
        catch (FileNotFoundException)
        {
            return ValueTask.FromResult<Protos.Documents.Shared.Document?>(null);
        }
    }

    public ValueTask<IEnumerable<Protos.Documents.Shared.Document>> GetAllAsync(string templateName)
    {
        var files = Directory.GetFiles(GetDirectoryPath(templateName));

        var documents = new Protos.Documents.Shared.Document[files.Length];

        for (var i = 0; i < files.Length; i++)
        {
            try
            {
                using var fileStream = File.OpenRead(files[i]);
                documents[i] = _serializer.Deserialize<Protos.Documents.Shared.Document>(fileStream);
            } catch (IOException)
            {
                _logger.LogWarning("Could not read document {DocumentId} for template {TemplateName}", files[i],
                    templateName);
            }
        }

        return ValueTask.FromResult<IEnumerable<Protos.Documents.Shared.Document>>(documents);
    }

    public ValueTask DeleteAsync(string templateName, ulong documentId)
    {
        try
        {
            File.Delete(GetFilePath(templateName, documentId));
        }
        catch (IOException)
        {
            _logger.LogWarning("Could not delete document {DocumentId} for template {TemplateName}", documentId,
                templateName);
        }

        return ValueTask.CompletedTask;
    }


    public ValueTask InitializeForTemplateAsync(string templateName)
    {
        var directoryPath = GetDirectoryPath(templateName);
        try
        {
            Directory.CreateDirectory(directoryPath);
        }
        catch (IOException)
        {
            _logger.LogWarning("Directory {Directory} already exists", directoryPath);
        }

        return ValueTask.CompletedTask;
    }
}
