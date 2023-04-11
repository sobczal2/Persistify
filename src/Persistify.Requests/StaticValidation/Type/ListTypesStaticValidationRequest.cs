using Persistify.Dtos.Request.Type;
using Persistify.Requests.Common;

namespace Persistify.Requests.StaticValidation.Type;

public class ListTypesStaticValidationRequest : StaticValidationRequest<ListTypesRequestDto>
{
    public ListTypesStaticValidationRequest(ListTypesRequestDto dto) : base(dto)
    {
    }
}