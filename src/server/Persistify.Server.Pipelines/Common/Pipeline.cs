using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Persistify.Helpers.ErrorHandling;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Pipelines.Common;

public abstract class Pipeline<TContext, TRequest, TResponse>
    where TContext : class, IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly ILogger<Pipeline<TContext, TRequest, TResponse>> _logger;

    protected Pipeline(
        ILogger<Pipeline<TContext, TRequest, TResponse>> logger
    )

    {
        _logger = logger;
    }

    protected abstract IEnumerable<PipelineStage<TContext, TRequest, TResponse>> PipelineStages { get; }

    protected abstract TContext CreateContext(TRequest request);

    protected abstract TResponse CreateResponse(TContext context);

    public async ValueTask<TResponse> ProcessAsync(TRequest request)
    {
        if (request is null)
        {
            throw new ValidationException(typeof(TRequest).Name, "Request cannot be null");
        }

        var context = CreateContext(request);

        try
        {
            foreach (var pipelineStage in PipelineStages)
            {
                await pipelineStage.ProcessAsync(context);
            }
        }
        catch (Exception ex)
        {
            if (ex is ExceptionWithStatus exceptionWithStatus)
            {
                throw new RpcException(exceptionWithStatus.Status);
            }

            _logger.LogError(ex, "Error while processing pipeline");
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }

        return CreateResponse(context);
    }
}
