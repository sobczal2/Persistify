namespace Persistify.Pipelines.Common;

public abstract class PipelineStage<TContext>
    where TContext : PipelineContext
{
    public virtual Task BeforeProcess(TContext context)
    {
        // TContextODO: emit start event
        return Task.CompletedTask;
    }

    public abstract Task Process(TContext context);

    public virtual Task AfterProcess(TContext context)
    {
        // TODO: emit end event
        return Task.CompletedTask;
    }
}
