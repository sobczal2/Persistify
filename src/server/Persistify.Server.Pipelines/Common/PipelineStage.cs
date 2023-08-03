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
    public abstract ValueTask ProcessAsync(TContext context);
}
