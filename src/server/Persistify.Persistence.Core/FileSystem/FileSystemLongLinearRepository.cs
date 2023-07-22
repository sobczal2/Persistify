using Persistify.Persistence.Core.Abstractions;

namespace Persistify.Persistence.Core.FileSystem;

public class FileSystemLongLinearRepository : ILongLinearRepository
{
    private const long EmptyValue = -1L;
    private readonly string _filePath;
    private readonly Mutex _mutex;

    public FileSystemLongLinearRepository(string filePath)
    {
        _filePath = filePath;
        _mutex = new Mutex();

        if (!File.Exists(_filePath))
        {
            File.Create(_filePath).Dispose();
        }
    }

    public ValueTask WriteAsync(int id, long value)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        _mutex.WaitOne();
        try
        {
            return WriteInternalAsync(id, value);
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }

    private ValueTask WriteInternalAsync(int id, long value)
    {
        using var binaryWriter = new BinaryWriter(File.Open(_filePath, FileMode.Open));

        if(binaryWriter.BaseStream.Length < (id + 1) * sizeof(long))
        {
            binaryWriter.Seek(0, SeekOrigin.End);
            binaryWriter.Write(new byte[(id + 1) * sizeof(long) - binaryWriter.BaseStream.Length]);
        }

        binaryWriter.Seek(id * sizeof(long), SeekOrigin.Begin);
        binaryWriter.Write(value);
        return ValueTask.CompletedTask;
    }

    public ValueTask<long?> ReadAsync(int id)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        _mutex.WaitOne();
        try
        {
            return ReadInternalAsync(id);
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }

    private ValueTask<long?> ReadInternalAsync(int id)
    {
        using var binaryReader = new BinaryReader(File.Open(_filePath, FileMode.Open));

        if (binaryReader.BaseStream.Length < (id + 1) * sizeof(long))
        {
            return ValueTask.FromResult<long?>(null);
        }

        binaryReader.BaseStream.Seek(id * sizeof(long), SeekOrigin.Begin);

        var value = binaryReader.ReadInt64();
        if (value == EmptyValue)
        {
            return ValueTask.FromResult<long?>(null);
        }

        return ValueTask.FromResult<long?>(value);
    }

    public ValueTask<IEnumerable<long>> ReadAllAsync()
    {
        _mutex.WaitOne();
        try
        {
            return ReadAllInternalAsync();
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }

    private ValueTask<IEnumerable<long>> ReadAllInternalAsync()
    {
        using var binaryReader = new BinaryReader(File.Open(_filePath, FileMode.Open));
        var values = new List<long>((int)(binaryReader.BaseStream.Length / sizeof(long)));
        while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
        {
            values.Add(binaryReader.ReadInt64());
        }

        return new ValueTask<IEnumerable<long>>(values);
    }

    public ValueTask RemoveAsync(int id)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        _mutex.WaitOne();
        try
        {
            return RemoveInternalAsync(id);
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }

    private ValueTask RemoveInternalAsync(int id)
    {
        using var binaryWriter = new BinaryWriter(File.Open(_filePath, FileMode.Open));

        if (binaryWriter.BaseStream.Length < (id + 1) * sizeof(long))
        {
            return ValueTask.CompletedTask;
        }

        binaryWriter.Seek(id * sizeof(long), SeekOrigin.Begin);
        binaryWriter.Write(EmptyValue);
        return ValueTask.CompletedTask;
    }
}
