using System;

namespace Persistify.Server.ErrorHandling.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string propertyName, string message)
        : base(message)
    {
        PropertyName = propertyName;
    }

    public string PropertyName { get; }
}
