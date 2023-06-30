namespace Persistify.Pipelines.Common;

public abstract class Pipeline<TContext, TRequest, TResponse>
    where TContext : PipelineContext
    where TRequest : class
    where TResponse : class
{
    private readonly PipelineStage<TContext>[] _pipelineStages;

    public Pipeline(PipelineStage<TContext>[] pipelineStages)
    {
        _pipelineStages = pipelineStages;
    }

    protected abstract TContext CreateContext(TRequest request);
    protected abstract TResponse CreateResponse(TContext context);

    public async Task<TResponse> Process(TRequest request)
    {
        var context = CreateContext(request);

        foreach (var stage in _pipelineStages)
        {
            await stage.BeforeProcess(context);
            await stage.Process(context);
            await stage.AfterProcess(context);
        }

        return CreateResponse(context);
    }
}
