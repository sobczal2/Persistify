using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Persistance.KeyValue;

public class FileSystemKeyValueStorage : IKeyValueStorage
{
    private readonly string _path;

    public FileSystemKeyValueStorage(IOptions<StorageSettings> storageSettings)
    {
        _path = storageSettings.Value.KeyValueStoragePath;
    }

    public ValueTask SetAsync<TValue>(string key, TValue value)
    {
        var filePath = GetFilePath(key);
        var json = JsonSerializer.Serialize(value);
        File.WriteAllText(filePath, json);
        return ValueTask.CompletedTask;
    }

    public async ValueTask<TValue?> GetAsync<TValue>(string key)
    {
        var filePath = GetFilePath(key);
        if (!File.Exists(filePath))
        {
            return default;
        }

        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<TValue>(json);
    }

    public ValueTask DeleteAsync(string key)
    {
        var filePath = GetFilePath(key);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return ValueTask.CompletedTask;
    }

    private string GetFilePath(string key)
    {
        return Path.Combine(_path, $"{key}.json");
    }
}
