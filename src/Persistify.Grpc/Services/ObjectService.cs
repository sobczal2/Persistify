using System.Threading.Tasks;
using Grpc.Core;
using Mediator;
using Persistify.ExternalDtos.Request.Object;
using Persistify.ExternalDtos.Response.Object;
using Persistify.ExternalDtos.Response.Shared;
using Persistify.Grpc.ProtoMappers;
using Persistify.Requests.Core.Objects;

namespace Persistify.Grpc.Services;

public class ObjectService : ObjectsService.ObjectsServiceBase
{
    private readonly IProtoMapper<IndexObjectRequestProto, IndexObjectRequestDto> _indexObjectRequestProtoMapper;

    private readonly IProtoMapper<IndexObjectSuccessResponseProto, IndexObjectSuccessResponseDto>
        _indexObjectSuccessResponseProtoMapper;

    private readonly IProtoMapper<InternalErrorResponseProto, InternalErrorResponseDto>
        _internalErrorResponseProtoMapper;

    private readonly IMediator _mediator;

    private readonly IProtoMapper<SearchObjectsRequestProto, SearchObjectsRequestDto> _searchObjectsRequestProtoMapper;

    private readonly IProtoMapper<SearchObjectsSuccessResponseProto, SearchObjectsSuccessResponseDto>
        _searchObjectsSuccessResponseProtoMapper;

    private readonly IProtoMapper<ValidationErrorResponseProto, ValidationErrorResponseDto>
        _validationErrorResponseProtoMapper;

    public ObjectService(
        IMediator mediator,
        IProtoMapper<ValidationErrorResponseProto, ValidationErrorResponseDto> validationErrorResponseProtoMapper,
        IProtoMapper<InternalErrorResponseProto, InternalErrorResponseDto> internalErrorResponseProtoMapper,
        IProtoMapper<IndexObjectRequestProto, IndexObjectRequestDto> indexObjectRequestProtoMapper,
        IProtoMapper<IndexObjectSuccessResponseProto, IndexObjectSuccessResponseDto>
            indexObjectSuccessResponseProtoMapper,
        IProtoMapper<SearchObjectsRequestProto, SearchObjectsRequestDto> searchObjectsRequestProtoMapper,
        IProtoMapper<SearchObjectsSuccessResponseProto, SearchObjectsSuccessResponseDto>
            searchObjectsSuccessResponseProtoMapper
    )
    {
        _mediator = mediator;
        _validationErrorResponseProtoMapper = validationErrorResponseProtoMapper;
        _internalErrorResponseProtoMapper = internalErrorResponseProtoMapper;
        _indexObjectRequestProtoMapper = indexObjectRequestProtoMapper;
        _indexObjectSuccessResponseProtoMapper = indexObjectSuccessResponseProtoMapper;
        _searchObjectsRequestProtoMapper = searchObjectsRequestProtoMapper;
        _searchObjectsSuccessResponseProtoMapper = searchObjectsSuccessResponseProtoMapper;
    }

    public override async Task<IndexObjectResponseProto> Index(IndexObjectRequestProto request,
        ServerCallContext context)
    {
        var requestDto = _indexObjectRequestProtoMapper.MapToDto(request);
        var coreRequest = new IndexObjectRequest(requestDto);
        var responseDto = await _mediator.Send(coreRequest, context.CancellationToken);
        return responseDto.Match(
            success => new IndexObjectResponseProto
            {
                Success = _indexObjectSuccessResponseProtoMapper.MapToProto(success)
            },
            validationError => new IndexObjectResponseProto
            {
                ValidationError = _validationErrorResponseProtoMapper.MapToProto(validationError)
            },
            internalError => new IndexObjectResponseProto
            {
                InternalError = _internalErrorResponseProtoMapper.MapToProto(internalError)
            }
        );
    }

    public override async Task<SearchObjectsResponseProto> Search(SearchObjectsRequestProto request,
        ServerCallContext context)
    {
        var requestDto = _searchObjectsRequestProtoMapper.MapToDto(request);
        var coreRequest = new SearchObjectsRequest(requestDto);
        var responseDto = await _mediator.Send(coreRequest, context.CancellationToken);
        return responseDto.Match(
            success => new SearchObjectsResponseProto
            {
                Success = _searchObjectsSuccessResponseProtoMapper.MapToProto(success)
            },
            validationError => new SearchObjectsResponseProto
            {
                ValidationError = _validationErrorResponseProtoMapper.MapToProto(validationError)
            },
            internalError => new SearchObjectsResponseProto
            {
                InternalError = _internalErrorResponseProtoMapper.MapToProto(internalError)
            }
        );
    }
}