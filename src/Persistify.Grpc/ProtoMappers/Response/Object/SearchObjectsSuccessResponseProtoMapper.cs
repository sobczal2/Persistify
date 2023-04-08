using System.Linq;
using Persistify.ExternalDtos.Common.Pagination;
using Persistify.ExternalDtos.Response.Object;

namespace Persistify.Grpc.ProtoMappers.Response.Object;

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