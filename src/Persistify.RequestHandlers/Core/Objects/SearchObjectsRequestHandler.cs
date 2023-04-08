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

public class SearchObjectsRequestHandler : CoreRequestHandler<SearchObjectsRequest, SearchObjectsSuccessResponseDto>
{
    private readonly IMediator _mediator;

    public SearchObjectsRequestHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async
        ValueTask<OneOf<SearchObjectsSuccessResponseDto, ValidationErrorResponseDto, InternalErrorResponseDto>> Handle(
            SearchObjectsRequest request, CancellationToken cancellationToken)
    {
        var validationErrors =
            (await _mediator.Send(new StaticValidateSearchObjectsRequest(request.Request),
                cancellationToken)).ToArray();

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