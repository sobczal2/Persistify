using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;

namespace Persistify.Server.Pipelines.Common;

public abstract class PipelineStage<TContext, TRequest, TResponse>
    where TContext : class, IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    public abstract string Name { get; }
    public abstract ValueTask<Result> ProcessAsync(TContext context);
    public abstract ValueTask<Result> RollbackAsync(TContext context);
}
