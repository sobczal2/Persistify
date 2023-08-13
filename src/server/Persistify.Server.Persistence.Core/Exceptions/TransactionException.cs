using System;

namespace Persistify.Server.Persistence.Core.Exceptions;

public class TransactionException : Exception
{
    public TransactionException() : base("Transaction error")
    {

    }

    public TransactionException(string message) : base(message)
    {

    }
}
