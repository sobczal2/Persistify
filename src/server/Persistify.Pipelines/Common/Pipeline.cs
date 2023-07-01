using Grpc.Core;
using Persistify.Helpers.ErrorHandling;

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

    public async ValueTask<TResponse> Process(TRequest request)
    {
        var context = CreateContext(request);

        foreach (var stage in _pipelineStages)
        {
            (await stage.BeforeProcess(context)).OnFailure(Throw);
            (await stage.Process(context)).OnFailure(Throw);
            (await stage.AfterProcess(context)).OnFailure(Throw);
        }

        return CreateResponse(context);
    }

    private static void Throw(Exception exception)
    {
        if (exception is ExceptionWithStatus exceptionWithStatus)
        {
            throw new RpcException(exceptionWithStatus.Status);
        }

        throw new RpcException(new Status(StatusCode.Internal, exception.Message));
    }
}
