using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Persistence.Core.Abstractions;

namespace Persistify.Persistence.Core.FileSystem
{
    public class FileSystemLongLinearRepository : ILongLinearRepository, IDisposable
    {
        private const long EmptyValue = -1L;
        private readonly FileStream _fileStream;
        private readonly SemaphoreSlim _mutex;

        public FileSystemLongLinearRepository(string filePath)
        {
            _fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            _mutex = new SemaphoreSlim(1, 1);
        }

        public async ValueTask WriteAsync(long id, long value)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            await _mutex.WaitAsync();
            try
            {
                await WriteInternalAsync(id, value);
            }
            finally
            {
                _mutex.Release();
            }
        }

        public async ValueTask<long?> ReadAsync(long id)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            await _mutex.WaitAsync();
            try
            {
                return await ReadInternalAsync(id);
            }
            finally
            {
                _mutex.Release();
            }
        }

        public async ValueTask<IEnumerable<(long Id, long Value)>> ReadAllAsync()
        {
            await _mutex.WaitAsync();
            try
            {
                return await ReadAllInternalAsync().ToListAsync();
            }
            finally
            {
                _mutex.Release();
            }
        }

        public async ValueTask RemoveAsync(long id)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            await _mutex.WaitAsync();
            try
            {
                await RemoveInternalAsync(id);
            }
            finally
            {
                _mutex.Release();
            }
        }

        public ValueTask<long> CountAsync()
        {
            return ValueTask.FromResult<long>(_fileStream.Length / sizeof(long));
        }

        private async ValueTask WriteInternalAsync(long id, long value)
        {
            if (_fileStream.Length < (id + 1) * sizeof(long))
            {
                _fileStream.SetLength((id + 1) * sizeof(long));
            }

            _fileStream.Position = id * sizeof(long);
            byte[] bytes = BitConverter.GetBytes(value);
            await _fileStream.WriteAsync(bytes, 0, bytes.Length);
            await _fileStream.FlushAsync();
        }

        private async ValueTask<long?> ReadInternalAsync(long id)
        {
            if (_fileStream.Length < (id + 1) * sizeof(long))
            {
                return null;
            }

            _fileStream.Position = id * sizeof(long);
            byte[] buffer = new byte[sizeof(long)];
            await _fileStream.ReadAsync(buffer, 0, buffer.Length);

            long value = BitConverter.ToInt64(buffer, 0);
            return value == EmptyValue ? null : (long?)value;
        }

        private async IAsyncEnumerable<(long, long)> ReadAllInternalAsync()
        {
            _fileStream.Position = 0;
            byte[] buffer = new byte[sizeof(long)];
            for (long id = 0; id < _fileStream.Length / sizeof(long); id++)
            {
                var read = await _fileStream.ReadAsync(buffer, 0, buffer.Length);

                if (read != buffer.Length)
                {
                    throw new InvalidOperationException();
                }

                long value = BitConverter.ToInt64(buffer, 0);
                if (value != EmptyValue)
                {
                    yield return (id, value);
                }
            }
        }

        private async ValueTask RemoveInternalAsync(long id)
        {
            if (_fileStream.Length < (id + 1) * sizeof(long))
            {
                return;
            }

            _fileStream.Position = id * sizeof(long);
            byte[] bytes = BitConverter.GetBytes(EmptyValue);
            await _fileStream.WriteAsync(bytes, 0, bytes.Length);
            await _fileStream.FlushAsync();
        }

        public void Dispose()
        {
            _mutex.Dispose();
            _fileStream.Dispose();
        }
    }
}
