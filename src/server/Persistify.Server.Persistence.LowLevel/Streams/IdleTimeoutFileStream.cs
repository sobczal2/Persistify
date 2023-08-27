using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Persistify.Server.Persistence.LowLevel.Streams;

public class IdleTimeoutFileStream : Stream
{
    private readonly string _filePath;
    private FileStream? _fileStream;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly object _lock;
    private readonly TimeSpan _idleFileTimeout;

    public IdleTimeoutFileStream(
        TimeSpan idleFileTimeout,
        string filePath
    )
    {
        _filePath = filePath;
        _lock = new object();
        _idleFileTimeout = idleFileTimeout;
    }

    private void EnsureFileStreamOpen()
    {
        lock (_lock)
        {
            _fileStream ??= new FileStream(_filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
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
                _fileStream?.Dispose();
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

    public override bool CanRead
    {
        get
        {
            EnsureFileStreamOpen();
            return _fileStream!.CanRead;
        }
    }

    public override bool CanSeek
    {
        get
        {
            EnsureFileStreamOpen();
            return _fileStream!.CanSeek;
        }
    }
    public override bool CanWrite
    {
        get
        {
            EnsureFileStreamOpen();
            return _fileStream!.CanWrite;
        }
    }

    public override long Length
    {
        get
        {
            EnsureFileStreamOpen();
            return _fileStream!.Length;
        }
    }

    public override long Position
    {
        get
        {
            EnsureFileStreamOpen();
            return _fileStream!.Position;
        }
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
}
