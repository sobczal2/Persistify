using Persistify.ExternalDtos.Common.Pagination;
using Persistify.ExternalDtos.Common.Types;
using Persistify.ExternalDtos.Response.Type;

namespace Persistify.Grpc.ProtoMappers.Response.Type;

public class
    ListTypesSuccessResponseProtoMapper : ProtoMapper<ListTypesSuccessResponseProto, ListTypesSuccessResponseDto>
{
    private readonly IProtoMapper<PaginationResponseProto, PaginationResponseDto> _paginationResponseProtoMapper;
    private readonly IProtoMapper<TypeDefinitionProto, TypeDefinitionDto> _typeDefinitionProtoMapper;

    public ListTypesSuccessResponseProtoMapper(
        IProtoMapper<PaginationResponseProto, PaginationResponseDto> paginationResponseProtoMapper,
        IProtoMapper<TypeDefinitionProto, TypeDefinitionDto> typeDefinitionProtoMapper
    )
    {
        _paginationResponseProtoMapper = paginationResponseProtoMapper;
        _typeDefinitionProtoMapper = typeDefinitionProtoMapper;
    }

    public override ListTypesSuccessResponseProto MapToProto(ListTypesSuccessResponseDto dto)
    {
        return new ListTypesSuccessResponseProto
        {
            TypeDefinitions = { _typeDefinitionProtoMapper.MapToProto(dto.TypeDefinitions) },
            PaginationResponse = _paginationResponseProtoMapper.MapToProto(dto.PaginationResponse)
        };
    }

    public override ListTypesSuccessResponseDto MapToDto(ListTypesSuccessResponseProto proto)
    {
        return new ListTypesSuccessResponseDto
        {
            TypeDefinitions = _typeDefinitionProtoMapper.MapToDtoRF(proto.TypeDefinitions),
            PaginationResponse = _paginationResponseProtoMapper.MapToDto(proto.PaginationResponse)
        };
    }
}