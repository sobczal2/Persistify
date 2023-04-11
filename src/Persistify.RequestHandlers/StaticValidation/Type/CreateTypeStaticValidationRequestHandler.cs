using Persistify.Dtos.Request.Type;
using Persistify.Dtos.Validators;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.StaticValidation.Type;

namespace Persistify.RequestHandlers.StaticValidation.Type;

public class
    CreateTypeStaticValidationRequestHandler : StaticValidationRequestHandler<CreateTypeStaticValidationRequest,
        CreateTypeRequestDto>
{
    public CreateTypeStaticValidationRequestHandler(IValidator<CreateTypeRequestDto> validator) : base(validator)
    {
    }
}