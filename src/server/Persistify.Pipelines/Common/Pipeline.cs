using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Persistify.Helpers.ErrorHandling;
using Serilog.Context;

namespace Persistify.Pipelines.Common;

public abstract class Pipeline<TContext, TRequest, TResponse>
    where TContext : class, IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly ILogger<Pipeline<TContext, TRequest, TResponse>> _logger;
    protected abstract PipelineStage<TContext, TRequest, TResponse>[] PipelineStages { get; }

    public Pipeline(
        ILogger<Pipeline<TContext, TRequest, TResponse>> logger
    )
    {
        _logger = logger;
    }

    protected abstract TContext CreateContext(TRequest request);

    protected abstract ValueTask WriteResponseAsync(TContext context);

    public async ValueTask<TResponse> ProcessAsync(TRequest request)
    {
        var context = CreateContext(request);

        for (var i = 0; i < PipelineStages.Length; i++)
        {
            using var _ = LogContext.PushProperty("Stage", PipelineStages[i].Name);

            var stageNumber = i;
            await (await PipelineStages[i].ProcessAsync(context)).OnFailure(async x =>
                await Throw(x, context, stageNumber));
        }

        await WriteResponseAsync(context);

        if (context.Response is not null)
        {
            return context.Response;
        }

        _logger.LogError("Response was not set in pipeline");
        await Throw(new InvalidOperationException("Response is null"), context, PipelineStages.Length - 1);

        throw new PipelineException();
    }

    private async ValueTask Throw(Exception exception, TContext context, int stageNumber)
    {
        var stage = PipelineStages[stageNumber];
        if (exception is ExceptionWithStatus exceptionWithStatus)
        {
            _logger.LogInformation("Pipeline stage {Stage} failed with status {Status}", stage.Name,
                exceptionWithStatus.Status);
            await RollbackPreviousStagesAsync(stageNumber - 1, context);
            throw new RpcException(exceptionWithStatus.Status);
        }

        _logger.LogError(exception, "Pipeline stage {Stage} failed with exception", stage.Name);
        await RollbackPreviousStagesAsync(stageNumber - 1, context);
        throw new RpcException(new Status(StatusCode.Internal, exception.Message));
    }

    private async ValueTask RollbackPreviousStagesAsync(int lastStageNumber, TContext context)
    {
        _logger.LogInformation("Rolling back pipeline stages");

        for (var i = lastStageNumber; i >= 0; i--)
        {
            using var _ = LogContext.PushProperty("Stage", PipelineStages[i].Name);

            var stage = PipelineStages[i];
            (await stage.RollbackAsync(context)).OnFailure(x =>
                _logger.LogError(x, "Rollback of stage {Stage} failed", stage.Name));
        }

        _logger.LogInformation("Pipeline stages rolled back");
    }
}
