using System;

namespace Persistify.Persistance.Exceptions;

public class PersistenceException : Exception
{
    public PersistenceException() : base("Persistence failed")
    {
    }

    public PersistenceException(string message) : base(message)
    {
    }

    public PersistenceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
