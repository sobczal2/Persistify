using System;

namespace Persistify.Server.Persistence.Core.Transactions.Exceptions;

public class NotAllowedForTransactionException : Exception
{
    public NotAllowedForTransactionException() : base("This operation is not allowed for transaction")
    {

    }
}
