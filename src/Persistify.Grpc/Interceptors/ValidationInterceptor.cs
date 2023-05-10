using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Persistify.Validators.Core;

namespace Persistify.Grpc.Interceptors;

public class ValidationInterceptor : Interceptor
{
    private readonly ILogger<ValidationInterceptor> _logger;

    public ValidationInterceptor(ILogger<ValidationInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> next)
    {
        try
        {
            return await next(request, context);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation failed");

            var status = new Status(StatusCode.InvalidArgument, "Validation failed.");
            var metadata = new Metadata { { "error-codes", string.Join(';', ex.Failures.Select(x => x.ErrorCode)) } };

            throw new RpcException(status, metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred");
            throw;
        }
    }
}