using System;

namespace Persistify.Server.Validation.Common;

public class ValidationException : Exception
{
    public ValidationException(string propertyName, string message)
        : base(message)
    {
        PropertyName = propertyName;
    }

    public string PropertyName { get; }
}
