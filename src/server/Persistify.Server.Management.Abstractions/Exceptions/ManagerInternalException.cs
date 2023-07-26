using System;

namespace Persistify.Server.Management.Abstractions.Exceptions;

public class ManagerInternalException : Exception
{
    public ManagerInternalException(string message) : base(message)
    {
    }

    public ManagerInternalException() : base("Manager internal exception")
    {
    }

    public ManagerInternalException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
