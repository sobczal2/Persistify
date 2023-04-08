using Persistify.ExternalDtos.Request.Type;
using Persistify.ExternalDtos.Validators;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.StaticValidation.Type;

namespace Persistify.RequestHandlers.StaticValidation.Type;

public class StaticValidateCreateTypeRequestHandler : StaticValidateRequestHandler<StaticValidateCreateTypeRequest, CreateTypeRequestDto>
{
    public StaticValidateCreateTypeRequestHandler(IValidator<CreateTypeRequestDto> validator) : base(validator)
    {
    }
}