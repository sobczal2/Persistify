using Persistify.Diagnostics.Attributes;
using Persistify.Diagnostics.Enums;
using Persistify.ExternalDtos.Request.Object;
using Persistify.ExternalDtos.Response.Object;
using Persistify.Requests.Common;

namespace Persistify.Requests.Core.Objects;

[PipelineStep(PipelineStepType.Core)]
public class SearchObjectsRequest : CoreRequest<SearchObjectsRequestDto, SearchObjectsSuccessResponseDto>
{
    public SearchObjectsRequest(SearchObjectsRequestDto request) : base(request)
    {
    }
}