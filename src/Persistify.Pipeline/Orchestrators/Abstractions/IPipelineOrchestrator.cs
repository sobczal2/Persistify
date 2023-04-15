using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Abstractions;

namespace Persistify.Pipeline.Orchestrators.Abstractions;

public interface IPipelineOrchestrator<in TContext, TRequest, TResponse>
    where TContext : IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    Task ExecuteAsync(TContext context);
}