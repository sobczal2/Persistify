using Persistify.Helpers.ErrorHandling;

namespace Persistify.Pipelines.Common;

public abstract class PipelineStage<TContext>
    where TContext : PipelineContext
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
