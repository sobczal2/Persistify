using Persistify.Diagnostics.Attributes;
using Persistify.Diagnostics.Enums;
using Persistify.ExternalDtos.Request.Object;
using Persistify.Requests.Common;

namespace Persistify.Requests.StaticValidation.Objects;
[PipelineStep(PipelineStepType.Validation)]
public class StaticValidateIndexObjectRequest : StaticValidateRequest<IndexObjectRequestDto>
{
    public StaticValidateIndexObjectRequest(IndexObjectRequestDto dto) : base(dto)
    {
    }
}