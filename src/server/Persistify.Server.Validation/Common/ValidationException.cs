using System;

namespace Persistify.Server.Validation.Common;

public class ValidationException : Exception
{
    public string PropertyName { get; }
    public ValidationException(string propertyName, string message)
        : base(message)
    {
        PropertyName = propertyName;
    }
}
