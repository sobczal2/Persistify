using Persistify.Dtos.Request.Object;
using Persistify.Dtos.Validators;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.StaticValidation.Objects;

namespace Persistify.RequestHandlers.StaticValidation.Objects;

public class SearchObjectsStaticValidationRequestHandler : StaticValidationRequestHandler<
    SearchObjectsStaticValidationRequest
    , SearchObjectsRequestDto>
{
    public SearchObjectsStaticValidationRequestHandler(IValidator<SearchObjectsRequestDto> validator) : base(validator)
    {
    }
}