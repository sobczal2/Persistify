using System;

namespace Persistify.Server.Validation.Common;

// TODO: Change when creating not grpc interface
public class ValidationException : Exception
{
    public string PropertyName { get; }
    public ValidationException(string propertyName, string message)
        : base(message)
    {
        PropertyName = propertyName;
    }
}
