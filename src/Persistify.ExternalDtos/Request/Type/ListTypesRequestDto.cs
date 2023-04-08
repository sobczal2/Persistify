using Persistify.ExternalDtos.Common.Pagination;

namespace Persistify.ExternalDtos.Request.Type;

public struct ListTypesRequestDto
{
    public PaginationRequestDto PaginationRequest { get; init; }
}