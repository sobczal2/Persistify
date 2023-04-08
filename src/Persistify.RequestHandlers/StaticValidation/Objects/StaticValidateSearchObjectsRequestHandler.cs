using Persistify.ExternalDtos.Request.Object;
using Persistify.ExternalDtos.Validators;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.StaticValidation.Objects;

namespace Persistify.RequestHandlers.StaticValidation.Objects;

public class StaticValidateSearchObjectsRequestHandler : StaticValidateRequestHandler<StaticValidateSearchObjectsRequest, SearchObjectsRequestDto>
{
    public StaticValidateSearchObjectsRequestHandler(IValidator<SearchObjectsRequestDto> validator) : base(validator)
    {
    }
}