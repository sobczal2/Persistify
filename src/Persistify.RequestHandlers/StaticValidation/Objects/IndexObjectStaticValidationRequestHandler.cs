using Persistify.Dtos.Request.Object;
using Persistify.Dtos.Validators;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.StaticValidation.Objects;

namespace Persistify.RequestHandlers.StaticValidation.Objects;

public class
    IndexObjectStaticValidationRequestHandler : StaticValidationRequestHandler<IndexObjectStaticValidationRequest,
        IndexObjectRequestDto>
{
    public IndexObjectStaticValidationRequestHandler(IValidator<IndexObjectRequestDto> validator) : base(validator)
    {
    }
}