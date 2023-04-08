using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using OneOf;
using Persistify.ExternalDtos.Request.Type;
using Persistify.ExternalDtos.Response.Shared;
using Persistify.ExternalDtos.Response.Type;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.Common;
using Persistify.Requests.Core.Type;
using Persistify.Requests.StaticValidation.Type;

namespace Persistify.RequestHandlers.Core.Type;

public class CreateTypeRequestHandler : CoreRequestHandler<CreateTypeRequest, CreateTypeSuccessResponseDto>
{
    private readonly IMediator _mediator;

    public CreateTypeRequestHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async
        ValueTask<OneOf<CreateTypeSuccessResponseDto, ValidationErrorResponseDto, InternalErrorResponseDto>> Handle(
            CreateTypeRequest request, CancellationToken cancellationToken)
    {
        var validationErrors =
            (await _mediator.Send(new StaticValidateCreateTypeRequest(request.Request), cancellationToken))
            .ToArray();

        if (validationErrors.Any())
            return new ValidationErrorResponseDto
            {
                Errors = validationErrors.ToArray()
            };

        return new ValidationErrorResponseDto
        {
            Errors = new[]
            {
                new ValidationErrorDto
                {
                    Field = "Hello",
                    Message = "World"
                }
            }
        };
    }
}