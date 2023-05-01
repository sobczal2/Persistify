using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Orchestrators.Abstractions;

public abstract partial class
    PipelineOrchestratorBase<TSelf, TContext, TRequest, TResponse> : IPipelineOrchestrator<TContext, TRequest,
        TResponse>
    where TSelf : PipelineOrchestratorBase<TSelf, TContext, TRequest, TResponse>
    where TContext : IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly ILogger<TSelf> _logger;
    private readonly ISubject<PipelineEventProto> _pipelineEventSubject;
    private readonly ImmutableArray<IPipelineMiddleware<TContext, TRequest, TResponse>> _middlewares;

    protected PipelineOrchestratorBase(
        IEnumerable<IPipelineMiddleware<TContext, TRequest, TResponse>> middlewares,
        // ReSharper disable once ContextualLoggerProblem
        ILogger<TSelf> logger,
        ISubject<PipelineEventProto> pipelineEventSubject)
    {
        _logger = logger;
        _pipelineEventSubject = pipelineEventSubject;
        _middlewares = middlewares.ToImmutableArray();
    }

    public async Task ExecuteAsync(TContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var i = 0;
        try
        {
            for (; i < _middlewares.Length; i++)
            {
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
        finally
        {
            var success = i == _middlewares.Length;
            _pipelineEventSubject.OnNext(new PipelineEventProto()
            {
                CorrelationId = context.CorrelationId.ToString(),
                PipelineName = GetType().Name.Replace("Orchestrator", string.Empty),
                Timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                DurationUs = (uint)stopwatch.Elapsed.Microseconds,
                Success = success,
                FaultedStepName = success
                    ? string.Empty
                    : GenericNameRegex().Replace(_middlewares[i].GetType().Name
                        .Replace("Middleware", string.Empty), string.Empty)
            });
        }
    }

    public (string name, IEnumerable<string> middlewareNames) GetPipelineInfo()
    {
        return (GetType().Name.Replace("Orchestrator", string.Empty),
            _middlewares.Select(m => m.GetType().Name.Replace("Middleware", string.Empty)));
    }

    [GeneratedRegex("[`0-9]")]
    private static partial Regex GenericNameRegex();
}