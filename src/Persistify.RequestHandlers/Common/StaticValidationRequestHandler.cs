using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using OneOf;
using Persistify.Dtos.Response.Shared;
using Persistify.Dtos.Validators;
using Persistify.Requests.Common;

namespace Persistify.RequestHandlers.Common;

public abstract class
    StaticValidationRequestHandler<TRequest, TDto> : IRequestHandler<TRequest,
        OneOf<ValidationOkResponseDto, ValidationErrorResponseDto>>
    where TRequest : StaticValidationRequest<TDto>
    where TDto : notnull
{
    private readonly IValidator<TDto> _validator;

    public StaticValidationRequestHandler(
        IValidator<TDto> validator
    )
    {
        _validator = validator;
    }

    public ValueTask<OneOf<ValidationOkResponseDto, ValidationErrorResponseDto>> Handle(TRequest request,
        CancellationToken cancellationToken)
    {
        var validationErrors = _validator.Validate(request.Dto).ToArray();
        if (validationErrors.Any())
            return new ValueTask<OneOf<ValidationOkResponseDto, ValidationErrorResponseDto>>(
                new ValidationErrorResponseDto
                {
                    Errors = validationErrors
                });

        return new ValueTask<OneOf<ValidationOkResponseDto, ValidationErrorResponseDto>>(new ValidationOkResponseDto());
    }
}