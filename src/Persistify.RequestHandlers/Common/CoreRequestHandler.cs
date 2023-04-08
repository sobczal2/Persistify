using System.Threading;
using System.Threading.Tasks;
using Mediator;
using OneOf;
using Persistify.ExternalDtos.Response.Shared;

namespace Persistify.RequestHandlers.Common;

public abstract class CoreRequestHandler<TRequest, TSuccessResponse> : IRequestHandler<TRequest,
    OneOf<TSuccessResponse, ValidationErrorResponseDto, InternalErrorResponseDto>>
    where TSuccessResponse : notnull
    where TRequest : IRequest<OneOf<TSuccessResponse, ValidationErrorResponseDto, InternalErrorResponseDto>>
{
    public abstract ValueTask<OneOf<TSuccessResponse, ValidationErrorResponseDto, InternalErrorResponseDto>> Handle(
        TRequest request, CancellationToken cancellationToken);
}