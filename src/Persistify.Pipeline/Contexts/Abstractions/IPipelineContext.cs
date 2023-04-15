namespace Persistify.Pipeline.Contexts.Abstractions;

public interface IPipelineContext<out TProtoRequest, TProtoResponse>
    where TProtoRequest : class
    where TProtoResponse : class
{
    TProtoRequest Request { get; }
    TProtoResponse Response { get; }
    PipelineStep PreviousPipelineStep { get; set; }
    public void SetResponse(TProtoResponse response);
}