using System;
using Grpc.Core;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.ErrorHandling.ExceptionHandlers;

public class RpcExceptionHandler : IExceptionHandler
{
    public void Handle(Exception exception)
    {
        switch (exception)
        {
            case ValidationException validationException:
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"{validationException.PropertyName}: {validationException.Message}"));
            case InsufficientPermissionException insufficientPermissionException:
                throw new RpcException(new Status(StatusCode.PermissionDenied,
                    insufficientPermissionException.Message));
        }
    }
}
