using System;

namespace Persistify.Server.Persistence.Core.Transactions.Exceptions;

public class TransactionStateCorruptedException : Exception
{
    public TransactionStateCorruptedException() : base("Transaction state corrupted")
    {

    }
}
