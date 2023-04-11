using Persistify.Dtos.Request.Type;
using Persistify.Requests.Common;

namespace Persistify.Requests.StaticValidation.Type;

public class CreateTypeStaticValidationRequest : StaticValidationRequest<CreateTypeRequestDto>
{
    public CreateTypeStaticValidationRequest(CreateTypeRequestDto dto) : base(dto)
    {
    }
}