using Persistify.Dtos.Common.Pagination;
using Persistify.Protos;

namespace Persistify.ProtoMappers.Common.Pagination;

public class PaginationRequestProtoMapper : ProtoMapper<PaginationRequestProto, PaginationRequestDto>
{
    public override PaginationRequestProto MapToProto(PaginationRequestDto dto)
    {
        return new PaginationRequestProto
        {
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize
        };
    }

    public override PaginationRequestDto MapToDto(PaginationRequestProto proto)
    {
        return new PaginationRequestDto
        {
            PageNumber = proto.PageNumber,
            PageSize = proto.PageSize
        };
    }
}