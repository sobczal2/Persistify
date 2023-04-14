using System.Text;
using OneOf;
using Persistify.Dtos.Common;
using Persistify.Storage.Common;

namespace Persistify.Storage;

public class FileSystemStorage : IStorage
{
    private readonly string _rootPath;
    private readonly Func<Stream, Stream>? _compress;
    private readonly Func<Stream, Stream>? _decompress;

    public FileSystemStorage(string rootPath, Func<Stream, Stream>? compress = null, Func<Stream, Stream>? decompress = null)
    {
        _rootPath = rootPath;
        _compress = compress;
        _decompress = decompress;
    }

    public async ValueTask<OneOf<StorageSuccess<EmptyDto>, StorageError>> SaveBlobAsync(string key, string data,
        bool overwrite = false, CancellationToken cancellationToken = default)
    {
        var existsResult = await ExistsBlobAsync(key);
        if (existsResult.IsT1)
            return new StorageError(existsResult.AsT1.Message);

        if (!overwrite && existsResult.IsT0 && existsResult.AsT0.Data)
            return new StorageError("File already exists");

        try
        {
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
            await using var fileStream = File.Create(Path.Combine(_rootPath, key));
            await using var compressedStream = _compress != null ? _compress(memoryStream) : memoryStream;
            await compressedStream.CopyToAsync(fileStream, cancellationToken);
            return new StorageSuccess<EmptyDto>(new EmptyDto());
        }
        catch (Exception e)
        {
            return new StorageError(e.Message);
        }
    }

    public async ValueTask<OneOf<StorageSuccess<string>, StorageError>> LoadBlobAsync(string key, CancellationToken cancellationToken = default)
    {
        var existsResult = await ExistsBlobAsync(key, cancellationToken);
        if (existsResult.IsT1)
            return new StorageError(existsResult.AsT1.Message);

        if (existsResult.IsT0 && !existsResult.AsT0.Data)
            return new StorageError("File does not exist");
        try
        {
            await using var fileStream = File.OpenRead(Path.Combine(_rootPath, key));
            await using var decompressedStream = _decompress != null ? _decompress(fileStream) : fileStream;
            using var memoryStream = new MemoryStream();
            await decompressedStream.CopyToAsync(memoryStream, cancellationToken);
            var data = Encoding.UTF8.GetString(memoryStream.ToArray());
            return new StorageSuccess<string>(data);
        }
        catch (Exception e)
        {
            return new StorageError(e.Message);
        }
    }

    public async ValueTask<OneOf<StorageSuccess<EmptyDto>, StorageError>> DeleteBlobAsync(string key, CancellationToken cancellationToken = default)
    {
        var existsResult = await ExistsBlobAsync(key, cancellationToken);
        if (existsResult.IsT1)
            return new StorageError(existsResult.AsT1.Message);

        if (existsResult.IsT0 && !existsResult.AsT0.Data)
            return new StorageError("File does not exist");

        File.Delete(Path.Combine(_rootPath, key));
        return new StorageSuccess<EmptyDto>(new EmptyDto());
    }

    public ValueTask<OneOf<StorageSuccess<bool>, StorageError>> ExistsBlobAsync(string key, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult<OneOf<StorageSuccess<bool>, StorageError>>(
            new StorageSuccess<bool>(File.Exists(Path.Combine(_rootPath, key))));
    }
}