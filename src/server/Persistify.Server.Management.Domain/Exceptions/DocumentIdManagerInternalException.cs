using System;

namespace Persistify.Server.Management.Domain.Exceptions;

public class DocumentIdManagerInternalException : Exception
{
    public DocumentIdManagerInternalException(string message) : base(message)
    {
    }

    public DocumentIdManagerInternalException() : base("DocumentIdManager internal error")
    {
    }

    public DocumentIdManagerInternalException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
