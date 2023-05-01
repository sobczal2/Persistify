using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Abstractions;

namespace Persistify.Pipeline.Middlewares.Abstractions;

public interface IPipelineMiddleware<in TContext, TRequest, TResponse>
    where TContext : IPipelineContext<TRequest, TResponse>
    where TResponse : class
    where TRequest : class
{
    Task InvokeAsync(TContext context);
}