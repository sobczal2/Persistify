﻿using Grpc.Core;
using Persistify.Helpers.ErrorHandling;

namespace Persistify.Server.Validation.Common;

public class ValidationException : ExceptionWithStatus
{
    public ValidationException(string property, string message)
        : base(new Status(StatusCode.InvalidArgument, $"{property}: {message}"))
    {
    }
}