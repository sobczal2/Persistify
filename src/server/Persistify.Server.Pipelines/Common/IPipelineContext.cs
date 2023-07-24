namespace Persistify.Server.Pipelines.Common;

public interface IPipelineContext<TRequest, TResponse>
{
    TRequest Request { get; set; }

    TResponse? Response { get; set; }
}
