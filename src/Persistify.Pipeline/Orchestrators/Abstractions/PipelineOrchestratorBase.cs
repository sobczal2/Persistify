using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Wrappers.Abstractions;

namespace Persistify.Pipeline.Orchestrators.Abstractions;

public abstract class
    PipelineOrchestratorBase<TContext, TRequest, TResponse> : IPipelineOrchestrator<TContext, TRequest, TResponse>
    where TContext : IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly IEnumerable<IPipelineMiddleware<TContext, TRequest, TResponse>> _middlewares;
    private readonly IEnumerable<IMiddlewareWrapper<TContext, TRequest, TResponse>> _wrappers;

    protected PipelineOrchestratorBase(
        IEnumerable<IMiddlewareWrapper<TContext, TRequest, TResponse>> wrappers,
        IEnumerable<IPipelineMiddleware<TContext, TRequest, TResponse>> middlewares)
    {
        _middlewares = middlewares;
        _wrappers = wrappers.ToList();
    }

    public async Task ExecuteAsync(TContext context)
    {
        foreach (var middleware in _middlewares)
            await Wrap(context, async pipelineContext => await middleware.InvokeAsync(pipelineContext));
    }

    private Task Wrap(TContext context, Func<TContext, Task> action)
    {
        var task = action(context);
        return _wrappers.Aggregate(task, (current, wrapper) => wrapper.Wrap(context, async () => { await current; }));
    }
}