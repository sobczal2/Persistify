using System;
using System.Diagnostics;
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

    public Pipeline(
        ILogger<Pipeline<TContext, TRequest, TResponse>> logger
    )
    {
        _logger = logger;
    }

    protected abstract PipelineStage<TContext, TRequest, TResponse>[] PipelineStages { get; }

    protected abstract TContext CreateContext(TRequest request);

    protected abstract ValueTask<TResponse> CreateResonse(TContext context);

    public async ValueTask<TResponse> ProcessAsync(TRequest request)
    {
        var context = CreateContext(request);
        var stopwatch = new Stopwatch();

        for (var i = 0; i < PipelineStages.Length; i++)
        {
            var stageNumber = i;
            var stageName = PipelineStages[i].Name;

            _logger.LogInformation("Processing pipeline stage {StageName}", stageName);
            stopwatch.Restart();

            await (await PipelineStages[i].ProcessAsync(context)).OnFailure(async x =>
                await Throw(x, context, stageNumber));

            // TODO: Remove when monitoring is implemented
            stopwatch.Stop();
            _logger.LogInformation("Pipeline stage {StageName} processed in {ElapsedMilliseconds} μs", stageName,
                stopwatch.Elapsed.TotalMicroseconds);
        }

        return await CreateResonse(context);
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
            var stage = PipelineStages[i];
            _logger.LogInformation("Rolling back stage {Stage}", stage.Name);

            (await stage.RollbackAsync(context)).OnFailure(x =>
                _logger.LogError(x, "Rollback of stage {Stage} failed", stage.Name));
        }

        _logger.LogInformation("Pipeline stages rolled back");
    }
}
