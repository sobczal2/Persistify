using Persistify.Diagnostics.Attributes;
using Persistify.Diagnostics.Enums;
using Persistify.ExternalDtos.Request.Type;
using Persistify.Requests.Common;

namespace Persistify.Requests.StaticValidation.Type;

[PipelineStep(PipelineStepType.Validation)]
public class StaticValidateCreateTypeRequest : StaticValidateRequest<CreateTypeRequestDto>
{
    public StaticValidateCreateTypeRequest(CreateTypeRequestDto dto) : base(dto)
    {
    }
}