using Persistify.ExternalDtos.Common.Pagination;

namespace Persistify.Grpc.ProtoMappers.Common.Pagination;

public class PaginationResponseProtoMapper : ProtoMapper<PaginationResponseProto, PaginationResponseDto>
{
    public override PaginationResponseProto MapToProto(PaginationResponseDto dto)
    {
        return new PaginationResponseProto
        {
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize,
            TotalItems = dto.TotalItems,
            TotalPages = dto.TotalPages
        };
    }

    public override PaginationResponseDto MapToDto(PaginationResponseProto proto)
    {
        return new PaginationResponseDto
        {
            PageNumber = proto.PageNumber,
            PageSize = proto.PageSize,
            TotalItems = proto.TotalItems,
            TotalPages = proto.TotalPages
        };
    }
}