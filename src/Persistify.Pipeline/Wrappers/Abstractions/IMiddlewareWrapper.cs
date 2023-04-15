using System;
using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Abstractions;

namespace Persistify.Pipeline.Wrappers.Abstractions;

public interface IMiddlewareWrapper<TContext, TRequest, TResponse>
    where TContext : IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    Task Wrap(TContext context, Func<Task> action);
}