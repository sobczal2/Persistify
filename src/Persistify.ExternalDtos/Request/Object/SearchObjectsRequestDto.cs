using Persistify.ExternalDtos.Common.Pagination;

namespace Persistify.ExternalDtos.Request.Object;

public class SearchObjectsRequestDto
{
    public string TypeName { get; init; } = default!;
    public string Query { get; init; } = default!;
    public PaginationRequestDto PaginationRequest { get; init; }
    public string[] FieldPaths { get; init; } = default!; // if field paths are not specified, all fields will be used
}