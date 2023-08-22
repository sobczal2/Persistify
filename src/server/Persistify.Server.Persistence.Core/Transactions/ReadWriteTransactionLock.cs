using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Server.Persistence.Core.Transactions.Exceptions;

namespace Persistify.Server.Persistence.Core.Transactions
{
    public class ReadWriteTransactionLock : IDisposable
    {
        private readonly HashSet<long> _readers;
        private long _writer;

        private readonly SemaphoreSlim _readSemaphoreSlim;
        private readonly SemaphoreSlim _writeSemaphoreSlim;

        public ReadWriteTransactionLock()
        {
            _readers = new HashSet<long>();
            _writer = 0;
            _readSemaphoreSlim = new SemaphoreSlim(1, 1);
            _writeSemaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public async ValueTask EnterReadLockAsync(long transactionId)
        {
            await _writeSemaphoreSlim.WaitAsync().ConfigureAwait(false);

            try
            {
                await _readSemaphoreSlim.WaitAsync().ConfigureAwait(false);
                _readers.Add(transactionId);
            }
            finally
            {
                _readSemaphoreSlim.Release();
                if (_readers.Count == 1)
                    _writeSemaphoreSlim.Release();
            }
        }

        public async ValueTask ExitReadLockAsync(long transactionId)
        {
            await _readSemaphoreSlim.WaitAsync().ConfigureAwait(false);

            try
            {
                _readers.Remove(transactionId);
                if (_readers.Count == 0)
                    _writeSemaphoreSlim.Release();
            }
            finally
            {
                _readSemaphoreSlim.Release();
            }
        }


        public async ValueTask EnterWriteLockAsync(long transactionId)
        {
            await _writeSemaphoreSlim.WaitAsync().ConfigureAwait(false);
            if (_writer == 0)
            {
                _writer = transactionId;
            }
            else
            {
                _writeSemaphoreSlim.Release();
                throw new TransactionStateCorruptedException();
            }
        }

        public ValueTask ExitWriteLockAsync(long transactionId)
        {
            if (_writer == transactionId)
            {
                _writer = 0;
                _writeSemaphoreSlim.Release();
            }
            else
            {
                throw new TransactionStateCorruptedException();
            }

            return ValueTask.CompletedTask;
        }

        public bool CanRead(long transactionId)
        {
            return _readers.Contains(transactionId);
        }

        public bool CanWrite(long transactionId)
        {
            return _writer == transactionId;
        }

        public void Dispose()
        {
            _readSemaphoreSlim.Dispose();
            _writeSemaphoreSlim.Dispose();
        }
    }
}
