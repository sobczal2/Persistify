using System.Collections.Generic;
using Mediator;
using Persistify.Diagnostics.Attributes;
using Persistify.Diagnostics.Enums;
using Persistify.ExternalDtos.Response.Shared;

namespace Persistify.Requests.Common;

[PipelineStep(PipelineStepType.Validation)]
public class StaticValidateRequest<TDto> : IRequest<IEnumerable<ValidationErrorDto>>
{
    public StaticValidateRequest(TDto dto)
    {
        Dto = dto;
    }

    public TDto Dto { get; }
}