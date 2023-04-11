using Mediator;
using Persistify.Diagnostics.Attributes;
using Persistify.Diagnostics.Enums;

namespace Persistify.Requests.Common;

[PipelineStep(PipelineStepType.Core)]
public abstract class
    CoreRequest<TProto, TResponse> : IRequest<TResponse>
{
    protected CoreRequest(TProto proto)
    {
        Proto = proto;
    }

    public TProto Proto { get; }
}