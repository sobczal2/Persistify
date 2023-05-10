using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Persistify.Storage;

public class FileSystemStorage : IStorage
{
    private readonly Func<Stream, Stream>? _compress;
    private readonly Func<Stream, Stream>? _decompress;
    private readonly string _rootPath;

    public FileSystemStorage(
        string rootPath,
        Func<Stream, Stream>? compress = null,
        Func<Stream, Stream>? decompress = null
    )
    {
        _rootPath = rootPath;
        _compress = compress;
        _decompress = decompress;
    }

    public async ValueTask SaveBlobAsync(
        string key,
        string data,
        CancellationToken cancellationToken = default
    )
    {
        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        await using var fileStream = File.Create(Path.Combine(_rootPath, key));
        await using var compressedStream =
            _compress != null ? _compress(memoryStream) : memoryStream;
        await compressedStream.CopyToAsync(fileStream, cancellationToken);
    }

    public async ValueTask<string> LoadBlobAsync(
        string key,
        CancellationToken cancellationToken = default
    )
    {
        await using var fileStream = File.OpenRead(Path.Combine(_rootPath, key));
        await using var decompressedStream =
            _decompress != null ? _decompress(fileStream) : fileStream;
        using var memoryStream = new MemoryStream();
        await decompressedStream.CopyToAsync(memoryStream, cancellationToken);
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    public ValueTask DeleteBlobAsync(string key, CancellationToken cancellationToken = default)
    {
        File.Delete(Path.Combine(_rootPath, key));
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> ExistsBlobAsync(
        string key,
        CancellationToken cancellationToken = default
    )
    {
        return ValueTask.FromResult(File.Exists(Path.Combine(_rootPath, key)));
    }
}