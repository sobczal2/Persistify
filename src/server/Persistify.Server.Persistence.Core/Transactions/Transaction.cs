using System.Threading;
using Persistify.Helpers.Locking;

namespace Persistify.Server.Persistence.Core.Transactions;

public class Transaction
{
    private static AsyncLocal<Transaction> _currentTransaction;
    private static long _lastTransactionId;
    private static GlobalLock _globalLock;

    static Transaction() {
        _currentTransaction = new AsyncLocal<Transaction>();
        _lastTransactionId = 0;
        var tmp = _lastTransactionId;
        _globalLock = new GlobalLock();
    }

    public static bool CanReadGlobal()
    {
        if (_currentTransaction.Value is null)
        {
            return false;
        }

        return _globalLock.CanRead(_currentTransaction.Value.Id);
    }

    public static bool CanWriteGlobal()
    {
        if (_currentTransaction.Value is null)
        {
            return false;
        }

        return _globalLock.CanWrite(_currentTransaction.Value.Id);
    }

    public long Id { get; private set; }

    public Transaction()
    {
        Id = Interlocked.Increment(ref _lastTransactionId);
    }
}
