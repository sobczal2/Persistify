using System;
using Grpc.Core;

namespace Persistify.Server.Validation.Common;

// TODO: Change when creating not grpc interface
public class ValidationException : RpcException
{
    public ValidationException(string property, string message) : base(new Status(StatusCode.InvalidArgument,
        $"{property}: {message}"))
    {
    }
}
