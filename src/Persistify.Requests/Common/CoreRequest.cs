using Mediator;
using OneOf;
using Persistify.Diagnostics.Attributes;
using Persistify.Diagnostics.Enums;
using Persistify.ExternalDtos.Response.Shared;

namespace Persistify.Requests.Common;

[PipelineStep(PipelineStepType.Core)]
public abstract class
    CoreRequest<TRequest, TSuccessResponse> : IRequest<
        OneOf<TSuccessResponse, ValidationErrorResponseDto, InternalErrorResponseDto>>
    where TRequest : notnull
    where TSuccessResponse : notnull
{
    public CoreRequest(TRequest request)
    {
        Request = request;
    }

    public TRequest Request { get; init; }
}