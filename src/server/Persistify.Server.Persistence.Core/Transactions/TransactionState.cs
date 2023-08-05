using System;
using System.Threading;

namespace Persistify.Server.Persistence.Core.Transactions;

public static class TransactionState
{
    private static readonly AsyncLocal<Transaction?> AsyncLocalTransaction = new();

    public static Transaction? Current
    {
        get => AsyncLocalTransaction.Value;
        set => AsyncLocalTransaction.Value = value;
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
