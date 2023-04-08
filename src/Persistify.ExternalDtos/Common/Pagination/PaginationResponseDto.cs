namespace Persistify.ExternalDtos.Common.Pagination;

public struct PaginationResponseDto
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
}