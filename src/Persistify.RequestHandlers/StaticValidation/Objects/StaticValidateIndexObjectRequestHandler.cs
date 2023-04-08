using Persistify.ExternalDtos.Request.Object;
using Persistify.ExternalDtos.Validators;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.StaticValidation.Objects;

namespace Persistify.RequestHandlers.StaticValidation.Objects;

public class StaticValidateIndexObjectRequestHandler : StaticValidateRequestHandler<StaticValidateIndexObjectRequest, IndexObjectRequestDto>
{
    public StaticValidateIndexObjectRequestHandler(IValidator<IndexObjectRequestDto> validator) : base(validator)
    {
    }
}