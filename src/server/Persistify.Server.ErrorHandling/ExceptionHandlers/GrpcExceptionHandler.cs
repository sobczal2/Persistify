using System;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Persistify.Server.ErrorHandling.Codes;
using Persistify.Server.ErrorHandling.Exceptions;

namespace Persistify.Server.ErrorHandling.ExceptionHandlers;

public class GrpcExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GrpcExceptionHandler> _logger;
    private readonly IPersistifyErrorCodeMapper<StatusCode> _persistifyErrorCodeMapper;

    public GrpcExceptionHandler(
        IPersistifyErrorCodeMapper<StatusCode> persistifyErrorCodeMapper,
        ILogger<GrpcExceptionHandler> logger
    )
    {
        _persistifyErrorCodeMapper = persistifyErrorCodeMapper;
        _logger = logger;
    }

    public void Handle(Exception exception)
    {
        _logger.LogDebug(exception, "Exception occurred");

        if (exception is not PersistifyException persistifyException)
        {
            throw new RpcException(new Status(StatusCode.Internal, "Weird internal error"));
        }

        var statusCode = _persistifyErrorCodeMapper.Map(persistifyException.ErrorCode);
        throw new RpcException(new Status(statusCode, persistifyException.FormattedMessage));
    }
}
