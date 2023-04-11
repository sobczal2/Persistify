using Mediator;
using OneOf;
using Persistify.Diagnostics.Attributes;
using Persistify.Diagnostics.Enums;
using Persistify.Dtos.Response.Shared;

namespace Persistify.Requests.Common;

[PipelineStep(PipelineStepType.StaticValidation)]
public abstract class StaticValidationRequest<TDto> : IRequest<OneOf<ValidationOkResponseDto, ValidationErrorResponseDto>>
{
    public StaticValidationRequest(TDto dto)
    {
        Dto = dto;
    }

    public TDto Dto { get; }
}