using System;
using Grpc.Core;

namespace Persistify.Server.ErrorHandling.Codes;

public class GrpcPersistifyErrorCodeMapper : IPersistifyErrorCodeMapper<StatusCode>
{
    public StatusCode Map(
        PersistifyErrorCode errorCode
    )
    {
        return errorCode switch
        {
            PersistifyErrorCode.StaticValidationFailure => StatusCode.InvalidArgument,
            PersistifyErrorCode.DynamicValidationFailure => StatusCode.InvalidArgument,
            PersistifyErrorCode.InsufficientPermission => StatusCode.PermissionDenied,
            PersistifyErrorCode.InternalFailure => StatusCode.Internal,
            PersistifyErrorCode.Unauthenticated => StatusCode.Unauthenticated,
            PersistifyErrorCode.NotFound => StatusCode.NotFound,
            PersistifyErrorCode.AlreadyExists => StatusCode.AlreadyExists,
            PersistifyErrorCode.FatalInternalFailure => StatusCode.Internal,
            _ => throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, null)
        };
    }
}
