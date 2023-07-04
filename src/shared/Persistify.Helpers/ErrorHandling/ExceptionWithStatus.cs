using System;
using Grpc.Core;

namespace Persistify.Helpers.ErrorHandling;

public class ExceptionWithStatus : Exception
{
    public Status Status { get; }

    public ExceptionWithStatus(Status status)
    {
        Status = status;
    }
}
