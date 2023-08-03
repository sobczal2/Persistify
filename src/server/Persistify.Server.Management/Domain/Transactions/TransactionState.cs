using System;
using System.Threading;

namespace Persistify.Server.Management.Domain.Transactions;

public static class TransactionState
{
    private static AsyncLocal<Transaction?> _current = new();

    public static Transaction? Current
    {
        get => _current.Value;
        set => _current.Value = value;
    }

    public static Transaction RequiredCurrent
    {
        get
        {
            var transaction = Current;
            if (transaction == null)
            {
                throw new InvalidOperationException("Transaction not available.");
            }

            return transaction;
        }
    }
}
