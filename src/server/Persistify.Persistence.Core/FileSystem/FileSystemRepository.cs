using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Persistence.Core.Abstractions;
using Persistify.Serialization;

namespace Persistify.Persistence.Core.FileSystem;

public class FileSystemRepository<T> : IRepository<T>, IDisposable, IPurgable
{
    private readonly string _mainFilePath;
    private readonly ILongLinearRepository _offsetsRepository;
    private readonly ILongLinearRepository _lengthsRepository;
    private readonly ISerializer _serializer;
    private FileStream _fileStream;
    private readonly SemaphoreSlim _semaphore;

    public FileSystemRepository(
        string mainFilePath,
        ILongLinearRepository offsetsRepository,
        ILongLinearRepository lengthsRepository,
        ISerializer serializer
    )
    {
        _mainFilePath = mainFilePath;
        _offsetsRepository = offsetsRepository;
        _lengthsRepository = lengthsRepository;
        _serializer = serializer;
        _fileStream = new FileStream(mainFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        _semaphore = new SemaphoreSlim(1, 1);
    }

    public async ValueTask WriteAsync(long id, T value)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        await _semaphore.WaitAsync();
        try
        {
            await WriteInternalAsync(id, value);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask<T?> ReadAsync(long id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        await _semaphore.WaitAsync();
        try
        {
            return await ReadInternalAsync(id);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async IAsyncEnumerable<T> ReadAllAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            await foreach (var value in ReadAllInternalAsync())
            {
                yield return value;
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask<long> CountAsync()
    {
        return await _offsetsRepository.CountAsync();
    }

    public async ValueTask<bool> ExistsAsync(long id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        await _semaphore.WaitAsync();
        try
        {
            return await ExistsInternalAsync(id);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask DeleteAsync(long id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        await _semaphore.WaitAsync();
        try
        {
            await RemoveInternalAsync(id);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async ValueTask WriteInternalAsync(long id, T value)
    {
        var offset = await _offsetsRepository.ReadAsync(id);
        var bytes = _serializer.Serialize(value);

        if (offset is null)
        {
            offset = _fileStream.Length;
            await _offsetsRepository.WriteAsync(id, offset.Value);
            await _lengthsRepository.WriteAsync(id, bytes.Length);

            _fileStream.Seek(offset.Value, SeekOrigin.Begin);
            await _fileStream.WriteAsync(bytes, 0, bytes.Length);
            await _fileStream.FlushAsync();

            return;
        }

        var previousLength = await _lengthsRepository.ReadAsync(id) ?? throw new InvalidOperationException();

        // previous length is greater or equal to current length
        if (previousLength >= bytes.Length)
        {
            _fileStream.Seek(offset.Value, SeekOrigin.Begin);
            await _fileStream.WriteAsync(bytes, 0, bytes.Length);
            await _fileStream.FlushAsync();
            await _lengthsRepository.WriteAsync(id, bytes.Length);

            return;
        }

        // previous length is less than current length so we need to write to the end of the file
        _fileStream.Seek(0, SeekOrigin.End);
        offset = _fileStream.Position;
        await _offsetsRepository.WriteAsync(id, offset.Value);
        await _lengthsRepository.WriteAsync(id, bytes.Length);

        _fileStream.Seek(offset.Value, SeekOrigin.Begin);
        await _fileStream.WriteAsync(bytes, 0, bytes.Length);
        await _fileStream.FlushAsync();
    }

    private async ValueTask<T?> ReadInternalAsync(long id)
    {
        var offset = await _offsetsRepository.ReadAsync(id);
        if (offset is null)
        {
            return default;
        }

        var length = await _lengthsRepository.ReadAsync(id) ?? throw new InvalidOperationException();
        _fileStream.Seek(offset.Value, SeekOrigin.Begin);
        var bytes = new byte[length];
        var read = await _fileStream.ReadAsync(bytes, 0, (int)length);
        if (read != length)
        {
            throw new InvalidOperationException();
        }

        return _serializer.Deserialize<T>(bytes);
    }

    private async IAsyncEnumerable<T> ReadAllInternalAsync()
    {
        var offsets = await _offsetsRepository.ReadAllAsync();
        var lengths = await _lengthsRepository.ReadAllAsync();

        var offsetEnumerator = offsets.GetEnumerator();
        var lengthEnumerator = lengths.GetEnumerator();

        try
        {
            while (offsetEnumerator.MoveNext() && lengthEnumerator.MoveNext())
            {
                var offset = offsetEnumerator.Current;
                var length = lengthEnumerator.Current;

                _fileStream.Seek(offset.Value, SeekOrigin.Begin);
                var bytes = new byte[length.Value];
                var read = await _fileStream.ReadAsync(bytes, 0, (int)length.Value);
                if (read != length.Value)
                {
                    throw new InvalidOperationException();
                }

                yield return _serializer.Deserialize<T>(bytes);
            }
        }
        finally
        {
            offsetEnumerator.Dispose();
            lengthEnumerator.Dispose();
        }
    }


    private async ValueTask<bool> ExistsInternalAsync(long id)
    {
        var offset = await _offsetsRepository.ReadAsync(id);
        return offset is not null;
    }

    private async ValueTask RemoveInternalAsync(long id)
    {
        var offset = await _offsetsRepository.ReadAsync(id);
        if (offset is null)
        {
            return;
        }

        await _offsetsRepository.RemoveAsync(id);
        await _lengthsRepository.RemoveAsync(id);
    }

    public async ValueTask PurgeAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            await PurgeInternalAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async ValueTask PurgeInternalAsync()
    {
        var offsets = await _offsetsRepository.ReadAllAsync();
        var lengths = await _lengthsRepository.ReadAllAsync();

        var offsetEnumerator = offsets.GetEnumerator();
        var lengthEnumerator = lengths.GetEnumerator();

        var tempFilePath = _mainFilePath + ".tmp";
        await using var tempFileStream =
            new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

        try
        {
            while (offsetEnumerator.MoveNext() && lengthEnumerator.MoveNext())
            {
                var offset = offsetEnumerator.Current;
                var length = lengthEnumerator.Current;

                _fileStream.Seek(offset.Value, SeekOrigin.Begin);
                var bytes = new byte[length.Value];
                var read = await _fileStream.ReadAsync(bytes, 0, (int)length.Value);
                if (read != length.Value)
                {
                    throw new InvalidOperationException();
                }

                await tempFileStream.WriteAsync(bytes, 0, read);

                await _offsetsRepository.WriteAsync(offset.Id, tempFileStream.Position - read);
            }
        }
        catch (Exception)
        {
            tempFileStream.Close();
            File.Delete(tempFilePath);
            throw;
        }
        finally
        {
            offsetEnumerator.Dispose();
            lengthEnumerator.Dispose();
        }


        await tempFileStream.FlushAsync();
        tempFileStream.Close();

        _fileStream.Close();
        File.Delete(_mainFilePath);
        File.Move(tempFilePath, _mainFilePath);

        _fileStream = new FileStream(_mainFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
    }

    public void Dispose()
    {
        _fileStream.Close();
    }
}
