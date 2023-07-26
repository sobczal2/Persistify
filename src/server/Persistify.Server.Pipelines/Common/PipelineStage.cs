using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Helpers.ErrorHandling;

namespace Persistify.Server.Pipelines.Common;

public abstract class PipelineStage<TContext, TRequest, TResponse>
    where TContext : class, IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly ILogger<PipelineStage<TContext, TRequest, TResponse>> _logger;
    public abstract string Name { get; }

    public PipelineStage(ILogger<PipelineStage<TContext, TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    public ValueTask<Result> TryProcessAsync(TContext context)
    {
        try { return ProcessAsync(context); }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception in pipeline stage {StageName}. This is a bug", Name);
            return new(e);
        }
    }
    public abstract ValueTask<Result> ProcessAsync(TContext context);
    public abstract ValueTask<Result> RollbackAsync(TContext context);
}
