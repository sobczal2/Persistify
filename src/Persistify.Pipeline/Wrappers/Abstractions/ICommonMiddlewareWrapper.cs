using Persistify.Pipeline.Contexts.Abstractions;

namespace Persistify.Pipeline.Wrappers.Abstractions;

public interface
    ICommonMiddlewareWrapper<TContext, TRequest, TResponse> : IMiddlewareWrapper<TContext, TRequest, TResponse>
    where TContext : IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
}