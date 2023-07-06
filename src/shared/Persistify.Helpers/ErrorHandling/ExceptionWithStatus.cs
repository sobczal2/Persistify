using System;
using Grpc.Core;

namespace Persistify.Helpers.ErrorHandling;

public class ExceptionWithStatus : Exception
{
    public ExceptionWithStatus(Status status)
    {
        Status = status;
    }

    public Status Status { get; }
}
