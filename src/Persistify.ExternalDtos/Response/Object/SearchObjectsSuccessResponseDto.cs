using Persistify.ExternalDtos.Common.Pagination;

namespace Persistify.ExternalDtos.Response.Object;

public class SearchObjectsSuccessResponseDto
{
    public string[] Items { get; init; } = default!;
    public PaginationResponseDto PaginationResponse { get; init; }
}