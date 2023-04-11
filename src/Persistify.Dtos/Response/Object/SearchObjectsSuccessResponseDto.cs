using Persistify.Dtos.Common.Pagination;

namespace Persistify.Dtos.Response.Object;

public class SearchObjectsSuccessResponseDto
{
    public string[] Items { get; init; } = default!;
    public PaginationResponseDto PaginationResponse { get; init; }
}