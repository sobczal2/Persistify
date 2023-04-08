using Persistify.Diagnostics.Attributes;
using Persistify.Diagnostics.Enums;
using Persistify.ExternalDtos.Request.Type;
using Persistify.ExternalDtos.Response.Type;
using Persistify.Requests.Common;

namespace Persistify.Requests.Core.Type;

[PipelineStep(PipelineStepType.Core)]
public class ListTypesRequest : CoreRequest<ListTypesRequestDto, ListTypesSuccessResponseDto>
{
    public ListTypesRequest(ListTypesRequestDto request) : base(request)
    {
    }
}