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

public class ListTypesRequestHandler : CoreRequestHandler<ListTypesRequest, ListTypesSuccessResponseDto>
{
    private readonly IMediator _mediator;

    public ListTypesRequestHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async
        ValueTask<OneOf<ListTypesSuccessResponseDto, ValidationErrorResponseDto, InternalErrorResponseDto>> Handle(
            ListTypesRequest request, CancellationToken cancellationToken)
    {
        var validationErrors =
            (await _mediator.Send(new StaticValidateListTypesRequest(request.Request), cancellationToken))
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