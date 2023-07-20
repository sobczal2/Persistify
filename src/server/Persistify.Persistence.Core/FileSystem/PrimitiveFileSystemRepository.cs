using Persistify.Persistence.Core.Abstractions;
using Persistify.Serialization;

namespace Persistify.Persistence.Core.FileSystem;

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

    private string GetFilePath(long id) => Path.Combine(_directoryPath, $"{id:x8}.bin");

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

    public ValueTask<IEnumerable<T>> ReadAllAsync()
    {
        var filePaths = Directory.GetFiles(_directoryPath);
        var values = new List<T>(filePaths.Length);
        values.AddRange(filePaths.Select(filePath => _serializer.Deserialize<T>(File.ReadAllBytes(filePath))));

        return new ValueTask<IEnumerable<T>>(values);
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
}
