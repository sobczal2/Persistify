using System;

namespace Persistify.Server.Management.Transactions.Exceptions;

public class TransactionStateCorruptedException : Exception
{
    public TransactionStateCorruptedException()
        : base("Transaction state corrupted")
    {
    }
}
