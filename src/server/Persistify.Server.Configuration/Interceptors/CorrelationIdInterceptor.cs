using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Persistify.Server.Configuration.Interceptors;

public class CorrelationIdInterceptor : Interceptor
{
    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var correlationId = context.RequestHeaders.GetValue("X-Correlation-ID");
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.ResponseTrailers.Add("X-Correlation-ID", correlationId);
        return base.UnaryServerHandler(request, context, continuation);
    }
}
