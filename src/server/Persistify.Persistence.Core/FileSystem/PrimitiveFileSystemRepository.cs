using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Persistify.Persistence.Core.Abstractions;
using Persistify.Serialization;

namespace Persistify.Persistence.Core.FileSystem;

/// <summary>
/// Doesn't support id > int.MaxValue
/// </summary>
/// <typeparam name="T"></typeparam>
public class PrimitiveFileSystemRepository<T> : IRepository<T>
{
    private readonly string _directoryPath;
    private readonly ISerializer _serializer;

    public PrimitiveFileSystemRepository(
        string directoryPath,
        ISerializer serializer
    )
    {
        _directoryPath = directoryPath;
        _serializer = serializer;

        if (!Directory.Exists(_directoryPath))
        {
            Directory.CreateDirectory(_directoryPath);
        }
    }

    public async ValueTask WriteAsync(long id, T value)
    {
        await File.WriteAllBytesAsync(GetFilePath(id), _serializer.Serialize(value));
    }

    public async ValueTask<T?> ReadAsync(long id)
    {
        var filePath = GetFilePath(id);
        if (!File.Exists(filePath))
        {
            return default;
        }

        return _serializer.Deserialize<T>(await File.ReadAllBytesAsync(filePath));
    }

    public async IAsyncEnumerable<T> ReadAllAsync()
    {
        var filePaths = Directory.GetFiles(_directoryPath);

        foreach (var filePath in filePaths)
        {
            yield return _serializer.Deserialize<T>(await File.ReadAllBytesAsync(filePath));
        }
    }

    public ValueTask<long> CountAsync()
    {
        return ValueTask.FromResult<long>(Directory.GetFiles(_directoryPath).Length);
    }

    public ValueTask<bool> ExistsAsync(long id)
    {
        return ValueTask.FromResult(File.Exists(GetFilePath(id)));
    }

    public ValueTask RemoveAsync(long id)
    {
        File.Delete(GetFilePath(id));
        return ValueTask.CompletedTask;
    }

    private string GetFilePath(long id)
    {
        return Path.Combine(_directoryPath, $"{id:x8}.bin");
    }
}
