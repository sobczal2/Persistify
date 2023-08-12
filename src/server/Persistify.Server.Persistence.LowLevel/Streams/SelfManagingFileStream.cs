using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Server.Persistence.LowLevel.Streams;

public class SelfManagingFileStream : Stream
{
    private readonly string _filePath;
    private FileStream? _fileStream;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly object _lock;
    private readonly TimeSpan _idleFileTimeout;

    public SelfManagingFileStream(
        IOptions<StorageSettings> storageSettings,
        string filePath
    )
    {
        _filePath = filePath;
        _lock = new object();
        _idleFileTimeout = storageSettings.Value.IdleFileTimeout;
    }

    private void EnsureFileStreamOpen()
    {
        lock (_lock)
        {
            _fileStream ??= new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            CancelCloseFileTask();
            ScheduleCloseFile();
        }
    }

    private void CancelCloseFileTask()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
    }

    private void ScheduleCloseFile()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        Task.Delay(_idleFileTimeout, _cancellationTokenSource.Token).ContinueWith(t =>
        {
            if (t.IsCanceled)
            {
                return;
            }

            lock (_lock)
            {
                _fileStream?.Close();
                _fileStream = null;
            }
        });
    }

    public override void Flush()
    {
        EnsureFileStreamOpen();
        _fileStream!.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        EnsureFileStreamOpen();
        return _fileStream!.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        EnsureFileStreamOpen();
        return _fileStream!.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        EnsureFileStreamOpen();
        _fileStream!.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        EnsureFileStreamOpen();
        _fileStream!.Write(buffer, offset, count);
    }

    public override bool CanRead => _fileStream?.CanRead ?? false;
    public override bool CanSeek => _fileStream?.CanSeek ?? false;
    public override bool CanWrite => _fileStream?.CanWrite ?? false;

    public override long Length => _fileStream?.Length ?? throw new InvalidOperationException("File stream is not open.");

    public override long Position
    {
        get => _fileStream?.Position ?? throw new InvalidOperationException("File stream is not open.");
        set
        {
            EnsureFileStreamOpen();
            _fileStream!.Position = value;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CancelCloseFileTask();
            _fileStream?.Dispose();
            _cancellationTokenSource?.Dispose();
        }
        base.Dispose(disposing);
    }

    public void Delete()
    {
        CancelCloseFileTask();
        _fileStream?.Dispose();
        _cancellationTokenSource?.Dispose();
        File.Delete(_filePath);
    }
}
