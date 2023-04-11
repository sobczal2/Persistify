using Persistify.Dtos.Request.Type;
using Persistify.Dtos.Validators;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.StaticValidation.Type;

namespace Persistify.RequestHandlers.StaticValidation.Type;

public class
    ListTypesStaticValidationRequestHandler : StaticValidationRequestHandler<ListTypesStaticValidationRequest,
        ListTypesRequestDto>
{
    public ListTypesStaticValidationRequestHandler(IValidator<ListTypesRequestDto> validator) : base(validator)
    {
        var result = validator.Validate(new ListTypesRequestDto());
    }
}