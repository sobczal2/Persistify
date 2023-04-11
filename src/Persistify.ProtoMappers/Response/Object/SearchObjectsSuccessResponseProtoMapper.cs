using Persistify.Dtos.Common.Pagination;
using Persistify.Dtos.Response.Object;
using Persistify.Protos;

namespace Persistify.ProtoMappers.Response.Object;

public class
    SearchObjectsSuccessResponseProtoMapper : ProtoMapper<SearchObjectsSuccessResponseProto,
        SearchObjectsSuccessResponseDto>
{
    private readonly IProtoMapper<PaginationResponseProto, PaginationResponseDto> _paginationResponseProtoMapper;

    public SearchObjectsSuccessResponseProtoMapper(
        IProtoMapper<PaginationResponseProto, PaginationResponseDto> paginationResponseProtoMapper)
    {
        _paginationResponseProtoMapper = paginationResponseProtoMapper;
    }

    public override SearchObjectsSuccessResponseProto MapToProto(SearchObjectsSuccessResponseDto dto)
    {
        return new SearchObjectsSuccessResponseProto
        {
            Items = { dto.Items },
            PaginationResponse = _paginationResponseProtoMapper.MapToProto(dto.PaginationResponse)
        };
    }

    public override SearchObjectsSuccessResponseDto MapToDto(SearchObjectsSuccessResponseProto proto)
    {
        return new SearchObjectsSuccessResponseDto
        {
            Items = proto.Items.ToArray(),
            PaginationResponse = _paginationResponseProtoMapper.MapToDto(proto.PaginationResponse)
        };
    }
}