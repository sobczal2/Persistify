using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Pipeline.Wrappers.Abstractions;

namespace Persistify.Pipeline.Wrappers.Common;

public class
    TimeLoggingMiddlewareWrapper<TContext, TRequest, TResponse> : ICommonMiddlewareWrapper<TContext, TRequest,
        TResponse> where TContext : IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly ILogger<TimeLoggingMiddlewareWrapper<TContext, TRequest, TResponse>> _logger;

    public TimeLoggingMiddlewareWrapper(ILogger<TimeLoggingMiddlewareWrapper<TContext, TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task Wrap(TContext context, Func<Task> action)
    {
        var stopwatch = Stopwatch.StartNew();
        await action();
        _logger.LogInformation($"Step: {context.PreviousPipelineStep} took {stopwatch.Elapsed.Microseconds} μs.");
    }
}