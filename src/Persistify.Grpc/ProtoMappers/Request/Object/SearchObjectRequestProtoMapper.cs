using Persistify.ExternalDtos.Common.Pagination;
using Persistify.ExternalDtos.Request.Object;

namespace Persistify.Grpc.ProtoMappers.Request.Object;

public class SearchObjectRequestProtoMapper : ProtoMapper<SearchObjectsRequestProto, SearchObjectsRequestDto>
{
    private readonly IProtoMapper<PaginationRequestProto, PaginationRequestDto> _paginationRequestProtoMapper;

    public SearchObjectRequestProtoMapper(
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