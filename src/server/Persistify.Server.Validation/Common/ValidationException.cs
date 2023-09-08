using System;

namespace Persistify.Server.Validation.Common;

// TODO: Change when creating not grpc interface
public class ValidationException : Exception
{
    public string Property { get; }
    public ValidationException(string property, string message)
        : base(message)
    {
        Property = property;
    }
}
