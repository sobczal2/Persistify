using System;

namespace Persistify.Pipeline.Contexts.Abstractions;

public interface IPipelineContext<out TProtoRequest, TProtoResponse>
    where TProtoRequest : class
    where TProtoResponse : class
{
    Guid CorrelationId { get; }
    TProtoRequest Request { get; }
    TProtoResponse Response { get; }
    public void SetResponse(TProtoResponse response);
}