namespace Persistify.Dtos.Common.Pagination;

public struct PaginationRequestDto
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}