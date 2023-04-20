using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Middlewares.Abstractions;

namespace Persistify.Pipeline.Orchestrators.Abstractions;

public abstract class
    PipelineOrchestratorBase<TSelf, TContext, TRequest, TResponse> : IPipelineOrchestrator<TContext, TRequest, TResponse>
    where TSelf : PipelineOrchestratorBase<TSelf, TContext, TRequest, TResponse>
    where TContext : IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly ILogger<TSelf> _logger;
    private readonly ImmutableArray<IPipelineMiddleware<TContext, TRequest, TResponse>> _middlewares;

    protected PipelineOrchestratorBase(
        IEnumerable<IPipelineMiddleware<TContext, TRequest, TResponse>> middlewares,
        ILogger<TSelf> logger
        )
    {
        _logger = logger;
        _middlewares = middlewares.ToImmutableArray();
    }

    public async Task ExecuteAsync(TContext context)
    {
        for (var i = 0; i < _middlewares.Length; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            await _middlewares[i].InvokeAsync(context);
            var middlewareType = _middlewares[i].GetType();
            _logger.LogInformation(
                "[Id: {corellationId}] Pipeline step {middlewareNumber}: {middlewareName}({middlewareStepType}) took {microseconds} us.",
                context.CorrelationId,
                i + 1,
                middlewareType.Name,
                middlewareType.GetPipelineStep(),
                stopwatch.Elapsed.Microseconds);
            
        }
    }
}