using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Persistify.ExternalDtos.Response.Shared;
using Persistify.ExternalDtos.Validators;
using Persistify.Requests.Common;

namespace Persistify.RequestHandlers.Common;

public class
    StaticValidateRequestHandler<TRequest, TDto> : IRequestHandler<TRequest, IEnumerable<ValidationErrorDto>>
        where TRequest : StaticValidateRequest<TDto>
{
    private readonly IValidator<TDto> _validator;

    public StaticValidateRequestHandler(
        IValidator<TDto> validator
    )
    {
        _validator = validator;
    }
    
    public ValueTask<IEnumerable<ValidationErrorDto>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(_validator.Validate(request.Dto));
    }
}