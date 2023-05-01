using System;
using Persistify.Pipeline.Exceptions;

namespace Persistify.Pipeline.Contexts.Abstractions;

public abstract class
    PipelineContextBase<TProtoRequest, TProtoResponse> : IPipelineContext<TProtoRequest, TProtoResponse>
    where TProtoRequest : class
    where TProtoResponse : class
{
    protected readonly TProtoRequest RequestField;
    protected TProtoResponse? ResponseField;

    protected PipelineContextBase(TProtoRequest request)
    {
        RequestField = request;
        CorrelationId = Guid.NewGuid();
    }

    public Guid CorrelationId { get; }
    public TProtoRequest Request => RequestField;
    public TProtoResponse Response => ResponseField ?? throw new ResponseNotYetReadyException();

    public void SetResponse(TProtoResponse response)
    {
        if (ResponseField != null)
            throw new ResponseAlreadySetException();

        ResponseField = response;
    }
}