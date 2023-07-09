using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;

namespace Persistify.Pipelines.Common;

public abstract class PipelineStage<TContext, TRequest, TResponse>
    where TContext : class, IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    public virtual ValueTask<Result> BeforeProcess(TContext context)
    {
        // TODO: emit start event
        return ValueTask.FromResult(Result.Success);
    }

    public abstract ValueTask<Result> Process(TContext context);

    public virtual ValueTask<Result> AfterProcess(TContext context)
    {
        // TODO: emit end event
        return ValueTask.FromResult(Result.Success);
    }
}
