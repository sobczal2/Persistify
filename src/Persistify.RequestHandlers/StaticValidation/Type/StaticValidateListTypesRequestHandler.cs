using Persistify.ExternalDtos.Request.Type;
using Persistify.ExternalDtos.Validators;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.StaticValidation.Type;

namespace Persistify.RequestHandlers.StaticValidation.Type;

public class StaticValidateListTypesRequestHandler : StaticValidateRequestHandler<StaticValidateListTypesRequest, ListTypesRequestDto>
{
    public StaticValidateListTypesRequestHandler(IValidator<ListTypesRequestDto> validator) : base(validator)
    {
    }
}