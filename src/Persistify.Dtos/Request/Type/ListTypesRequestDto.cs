using Persistify.Dtos.Common.Pagination;

namespace Persistify.Dtos.Request.Type;

public struct ListTypesRequestDto
{
    public PaginationRequestDto PaginationRequest { get; init; }
}