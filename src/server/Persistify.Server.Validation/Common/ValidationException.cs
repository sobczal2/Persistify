using System;
using Grpc.Core;

namespace Persistify.Server.Validation.Common;

public class ValidationException : RpcException
{
    public ValidationException(string property, string message) : base(new Status(StatusCode.InvalidArgument,
        $"{property}: {message}"))
    {
    }
}
