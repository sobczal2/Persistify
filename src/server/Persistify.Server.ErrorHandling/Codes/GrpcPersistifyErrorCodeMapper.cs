using System;
using Grpc.Core;

namespace Persistify.Server.ErrorHandling.Codes;

public class GrpcPersistifyErrorCodeMapper : IPersistifyErrorCodeMapper<StatusCode>
{
    public StatusCode Map(PersistifyErrorCode errorCode) => errorCode switch
    {
        PersistifyErrorCode.StaticValidationFailure => StatusCode.InvalidArgument,
        PersistifyErrorCode.DynamicValidationFailure => StatusCode.InvalidArgument,
        PersistifyErrorCode.InsufficientPermission => StatusCode.PermissionDenied,
        PersistifyErrorCode.InternalFailure => StatusCode.Internal,
        PersistifyErrorCode.Unauthenticated => StatusCode.Unauthenticated,
        _ => throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, null)
    };
}
