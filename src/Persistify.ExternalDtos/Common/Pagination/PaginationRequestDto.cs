namespace Persistify.ExternalDtos.Common.Pagination;

public struct PaginationRequestDto
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}