using Persistify.Dtos.Common.Pagination;
using Persistify.Dtos.Common.Types;

namespace Persistify.Dtos.Response.Type;

public class ListTypesSuccessResponseDto
{
    public TypeDefinitionDto[] TypeDefinitions { get; init; } = default!;
    public PaginationResponseDto PaginationResponse { get; init; }
}