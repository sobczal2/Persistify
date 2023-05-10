using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Abstractions;

namespace Persistify.Pipeline.Orchestrators.Abstractions;

public interface IPipelineOrchestrator<in TContext, TRequest, TResponse>
    : IPipelineOrchestrator
    where TContext : IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    Task ExecuteAsync(TContext context);
}

public interface IPipelineOrchestrator
{
    (string name, IEnumerable<string> middlewareNames) GetPipelineInfo();
}