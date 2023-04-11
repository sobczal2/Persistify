using Persistify.Dtos.Request.Object;
using Persistify.Requests.Common;

namespace Persistify.Requests.StaticValidation.Objects;

public class SearchObjectsStaticValidationRequest : StaticValidationRequest<SearchObjectsRequestDto>
{
    public SearchObjectsStaticValidationRequest(SearchObjectsRequestDto dto) : base(dto)
    {
    }
}