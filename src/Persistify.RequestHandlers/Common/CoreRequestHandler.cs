using System;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Dtos.Response.Shared;
using Persistify.ProtoMappers;
using Persistify.Protos;

namespace Persistify.RequestHandlers.Common;

public abstract class CoreRequestHandler<TRequest, TResponse, TRequestProto, TRequestDto> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    public CoreRequestHandler(IMediator mediator, IProtoMapper<ValidationErrorResponseProto, ValidationErrorResponseDto> validationErrorResponseProtoMapper, IProtoMapper<TRequestProto, TRequestDto> requestProtoMapper)
    {
        Mediator = mediator;
        ValidationErrorResponseProtoMapper = validationErrorResponseProtoMapper;
        _requestProtoMapper = requestProtoMapper;
    }

    protected IMediator Mediator { get; }

    protected readonly IProtoMapper<ValidationErrorResponseProto, ValidationErrorResponseDto> ValidationErrorResponseProtoMapper;

    private readonly IProtoMapper<TRequestProto, TRequestDto> _requestProtoMapper;

    public abstract ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    
    protected TRequestDto MapToDto(TRequestProto request)
    {
        return _requestProtoMapper.MapToDto(request);
    }
}