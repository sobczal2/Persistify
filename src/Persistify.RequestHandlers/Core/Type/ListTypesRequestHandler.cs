using System;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Persistify.Dtos.Common.Pagination;
using Persistify.Dtos.Common.Types;
using Persistify.Dtos.Request.Type;
using Persistify.Dtos.Response.Shared;
using Persistify.ProtoMappers;
using Persistify.Protos;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.Core.Type;
using Persistify.Requests.StaticValidation.Type;
using Persistify.Requests.StoreManipulation.Types;

namespace Persistify.RequestHandlers.Core.Type;

public class ListTypesRequestHandler : CoreRequestHandler<ListTypesRequest, ListTypesResponseProto,
    ListTypesRequestProto, ListTypesRequestDto>
{
    private readonly IProtoMapper<TypeDefinitionProto, TypeDefinitionDto> _typeDefinitionProtoMapper;
    private readonly IProtoMapper<PaginationResponseProto, PaginationResponseDto> _paginationResponseProtoMapper;

    public ListTypesRequestHandler(
        IMediator mediator,
        IProtoMapper<ValidationErrorResponseProto, ValidationErrorResponseDto> validationErrorResponseProtoMapper,
        IProtoMapper<ListTypesRequestProto, ListTypesRequestDto> requestProtoMapper,
        IProtoMapper<TypeDefinitionProto, TypeDefinitionDto> typeDefinitionProtoMapper,
        IProtoMapper<PaginationResponseProto, PaginationResponseDto> paginationResponseProtoMapper
        ) : base(mediator, validationErrorResponseProtoMapper, requestProtoMapper)
    {
        _typeDefinitionProtoMapper = typeDefinitionProtoMapper;
        _paginationResponseProtoMapper = paginationResponseProtoMapper;
    }

    public override async ValueTask<ListTypesResponseProto> Handle(ListTypesRequest request,
        CancellationToken cancellationToken)
    {
        var requestDto = MapToDto(request.Proto);

        var validationResponse =
            await Mediator.Send(new ListTypesStaticValidationRequest(requestDto), cancellationToken);
        if (validationResponse.IsT1)
            return new ListTypesResponseProto
            {
                ValidationError = ValidationErrorResponseProtoMapper.MapToProto(validationResponse.AsT1)
            };

        var response = await Mediator.Send(new GetPagedTypesFromStoreRequest(requestDto.PaginationRequest),
            cancellationToken);
        
        return response.Match(
            success => new ListTypesResponseProto
            {
                Success = new ListTypesSuccessResponseProto()
                {
                    PaginationResponse = _paginationResponseProtoMapper.MapToProto(success.Data.PaginationResponse),
                    TypeDefinitions = { _typeDefinitionProtoMapper.MapToProtoRF(success.Data.Types) }
                }
            },
            error => new ListTypesResponseProto
            {
                InternalError = new InternalErrorResponseProto
                {
                    Message = error.Message
                }
            }
        );
    }
}