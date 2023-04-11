using Persistify.Dtos.Common.Pagination;
using Persistify.Dtos.Request.Type;
using Persistify.Protos;

namespace Persistify.ProtoMappers.Request.Type;

public class ListTypesRequestProtoMapper : ProtoMapper<ListTypesRequestProto, ListTypesRequestDto>
{
    private readonly IProtoMapper<PaginationRequestProto, PaginationRequestDto> _paginationRequestProtoMapper;

    public ListTypesRequestProtoMapper(
        IProtoMapper<PaginationRequestProto, PaginationRequestDto> paginationRequestProtoMapper)
    {
        _paginationRequestProtoMapper = paginationRequestProtoMapper;
    }

    public override ListTypesRequestProto MapToProto(ListTypesRequestDto dto)
    {
        return new ListTypesRequestProto
        {
            PaginationRequest = _paginationRequestProtoMapper.MapToProto(dto.PaginationRequest)
        };
    }

    public override ListTypesRequestDto MapToDto(ListTypesRequestProto proto)
    {
        return new ListTypesRequestDto
        {
            PaginationRequest = _paginationRequestProtoMapper.MapToDto(proto.PaginationRequest)
        };
    }
}