using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Persistify.Helpers.ErrorHandling;

namespace Persistify.Pipelines.Common;

public abstract class Pipeline<TContext, TRequest, TResponse>
    where TContext : class, IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly ILogger<Pipeline<TContext, TRequest, TResponse>> _logger;
    protected abstract IEnumerable<PipelineStage<TContext, TRequest, TResponse>> PipelineStages { get; }

    public Pipeline(
        ILogger<Pipeline<TContext, TRequest, TResponse>> logger
        )
    {
        _logger = logger;
    }

    protected abstract TContext CreateContext(TRequest request);

    public async ValueTask<TResponse> ProcessAsync(TRequest request)
    {
        var context = CreateContext(request);

        foreach (var stage in PipelineStages)
        {
            // ReSharper disable once ConvertToLocalFunction
            Action<Exception> onFailure = x => Throw(x, stage);

            (await stage.BeforeProcess(context)).OnFailure(onFailure);
            (await stage.Process(context)).OnFailure(onFailure);
            (await stage.AfterProcess(context)).OnFailure(onFailure);
        }

        if (context.Response is not null)
        {
            return context.Response;
        }

        _logger.LogError("Response was not set in pipeline");
        throw new InvalidOperationException("Response is null");
    }

    private void Throw(Exception exception, PipelineStage<TContext, TRequest, TResponse> stage)
    {
        if (exception is ExceptionWithStatus exceptionWithStatus)
        {
            _logger.LogInformation("Pipeline stage {Stage} failed with status {Status}", stage.GetType().Name, exceptionWithStatus.Status);
            throw new RpcException(exceptionWithStatus.Status);
        }

        _logger.LogError(exception, "Pipeline stage {Stage} failed with exception", stage.GetType().Name);
        throw new RpcException(new Status(StatusCode.Internal, exception.Message));
    }
}
