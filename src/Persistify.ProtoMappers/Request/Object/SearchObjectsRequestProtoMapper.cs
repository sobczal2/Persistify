using Persistify.Dtos.Common.Pagination;
using Persistify.Dtos.Request.Object;
using Persistify.Protos;

namespace Persistify.ProtoMappers.Request.Object;

public class SearchObjectsRequestProtoMapper : ProtoMapper<SearchObjectsRequestProto, SearchObjectsRequestDto>
{
    private readonly IProtoMapper<PaginationRequestProto, PaginationRequestDto> _paginationRequestProtoMapper;

    public SearchObjectsRequestProtoMapper(
        IProtoMapper<PaginationRequestProto, PaginationRequestDto> paginationRequestProtoMapper)
    {
        _paginationRequestProtoMapper = paginationRequestProtoMapper;
    }

    public override SearchObjectsRequestProto MapToProto(SearchObjectsRequestDto dto)
    {
        return new SearchObjectsRequestProto
        {
            TypeName = dto.TypeName,
            Query = dto.Query,
            PaginationRequest = _paginationRequestProtoMapper.MapToProto(dto.PaginationRequest)
        };
    }

    public override SearchObjectsRequestDto MapToDto(SearchObjectsRequestProto proto)
    {
        return new SearchObjectsRequestDto
        {
            TypeName = proto.TypeName,
            Query = proto.Query,
            PaginationRequest = _paginationRequestProtoMapper.MapToDto(proto.PaginationRequest)
        };
    }
}