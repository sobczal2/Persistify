using System;
using Grpc.Core;
using Persistify.Server.ErrorHandling.Codes;
using Persistify.Server.ErrorHandling.Exceptions;

namespace Persistify.Server.ErrorHandling.ExceptionHandlers;

public class GrpcExceptionHandler : IExceptionHandler
{
    private readonly IPersistifyErrorCodeMapper<StatusCode> _persistifyErrorCodeMapper;

    public GrpcExceptionHandler(
        IPersistifyErrorCodeMapper<StatusCode> persistifyErrorCodeMapper
    )
    {
        _persistifyErrorCodeMapper = persistifyErrorCodeMapper;
    }

    public void Handle(Exception exception)
    {
        if (exception is not PersistifyException persistifyException)
        {
            throw exception;
        }

        var statusCode = _persistifyErrorCodeMapper.Map(persistifyException.ErrorCode);
        throw new RpcException(new Status(statusCode, persistifyException.FormattedMessage));
    }
}
