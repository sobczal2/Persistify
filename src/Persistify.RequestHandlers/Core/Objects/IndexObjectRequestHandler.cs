using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using OneOf;
using Persistify.ExternalDtos.Request.Object;
using Persistify.ExternalDtos.Response.Object;
using Persistify.ExternalDtos.Response.Shared;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.Common;
using Persistify.Requests.Core.Objects;
using Persistify.Requests.StaticValidation.Objects;

namespace Persistify.RequestHandlers.Core.Objects;

public class IndexObjectRequestHandler : CoreRequestHandler<IndexObjectRequest, IndexObjectSuccessResponseDto>
{
    private readonly IMediator _mediator;

    public IndexObjectRequestHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async
        ValueTask<OneOf<IndexObjectSuccessResponseDto, ValidationErrorResponseDto, InternalErrorResponseDto>> Handle(
            IndexObjectRequest request, CancellationToken cancellationToken)
    {
        var validationErrors =
            (await _mediator.Send(new StaticValidateIndexObjectRequest(request.Request), cancellationToken))
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