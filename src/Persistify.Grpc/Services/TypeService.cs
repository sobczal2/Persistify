using System.Threading.Tasks;
using Grpc.Core;
using Mediator;
using Persistify.ExternalDtos.Request.Type;
using Persistify.ExternalDtos.Response.Shared;
using Persistify.ExternalDtos.Response.Type;
using Persistify.Grpc.ProtoMappers;
using Persistify.Requests.Core.Type;

namespace Persistify.Grpc.Services;

public class TypeService : TypesService.TypesServiceBase
{
    private readonly IProtoMapper<CreateTypeRequestProto, CreateTypeRequestDto> _createTypeRequestProtoMapper;

    private readonly IProtoMapper<CreateTypeSuccessResponseProto, CreateTypeSuccessResponseDto>
        _createTypeSuccessResponseProtoMapper;

    private readonly IProtoMapper<InternalErrorResponseProto, InternalErrorResponseDto>
        _internalErrorResponseProtoMapper;

    private readonly IProtoMapper<ListTypesRequestProto, ListTypesRequestDto> _listTypesRequestProtoMapper;

    private readonly IProtoMapper<ListTypesSuccessResponseProto, ListTypesSuccessResponseDto>
        _listTypesSuccessResponseProtoMapper;

    private readonly IMediator _mediator;

    private readonly IProtoMapper<ValidationErrorResponseProto, ValidationErrorResponseDto>
        _validationErrorResponseProtoMapper;

    public TypeService(
        IMediator mediator,
        IProtoMapper<ValidationErrorResponseProto, ValidationErrorResponseDto> validationErrorResponseProtoMapper,
        IProtoMapper<InternalErrorResponseProto, InternalErrorResponseDto> internalErrorResponseProtoMapper,
        IProtoMapper<CreateTypeRequestProto, CreateTypeRequestDto> createTypeRequestProtoMapper,
        IProtoMapper<CreateTypeSuccessResponseProto, CreateTypeSuccessResponseDto>
            createTypeSuccessResponseProtoMapper,
        IProtoMapper<ListTypesRequestProto, ListTypesRequestDto> listTypesRequestProtoMapper,
        IProtoMapper<ListTypesSuccessResponseProto, ListTypesSuccessResponseDto>
            listTypesSuccessResponseProtoMapper
    )
    {
        _mediator = mediator;
        _validationErrorResponseProtoMapper = validationErrorResponseProtoMapper;
        _internalErrorResponseProtoMapper = internalErrorResponseProtoMapper;
        _createTypeRequestProtoMapper = createTypeRequestProtoMapper;
        _createTypeSuccessResponseProtoMapper = createTypeSuccessResponseProtoMapper;
        _listTypesRequestProtoMapper = listTypesRequestProtoMapper;
        _listTypesSuccessResponseProtoMapper = listTypesSuccessResponseProtoMapper;
    }

    public override async Task<CreateTypeResponseProto> Create(CreateTypeRequestProto request,
        ServerCallContext context)
    {
        var requestDto = _createTypeRequestProtoMapper.MapToDto(request);
        var coreRequest = new CreateTypeRequest(requestDto);
        var responseDto = await _mediator.Send(coreRequest, context.CancellationToken);
        return responseDto.Match(
            success => new CreateTypeResponseProto
            {
                Success = _createTypeSuccessResponseProtoMapper.MapToProto(success)
            },
            validationError => new CreateTypeResponseProto
            {
                ValidationError = _validationErrorResponseProtoMapper.MapToProto(validationError)
            },
            internalError => new CreateTypeResponseProto
            {
                InternalError = _internalErrorResponseProtoMapper.MapToProto(internalError)
            }
        );
    }

    public override async Task<ListTypesResponseProto> List(ListTypesRequestProto request, ServerCallContext context)
    {
        var requestDto = _listTypesRequestProtoMapper.MapToDto(request);
        var coreRequest = new ListTypesRequest(requestDto);
        var responseDto = await _mediator.Send(coreRequest, context.CancellationToken);
        return responseDto.Match(
            success => new ListTypesResponseProto
            {
                Success = _listTypesSuccessResponseProtoMapper.MapToProto(success)
            },
            validationError => new ListTypesResponseProto
            {
                ValidationError = _validationErrorResponseProtoMapper.MapToProto(validationError)
            },
            internalError => new ListTypesResponseProto
            {
                InternalError = _internalErrorResponseProtoMapper.MapToProto(internalError)
            }
        );
    }
}