using Persistify.ExternalDtos.Common.Pagination;
using Persistify.ExternalDtos.Common.Types;

namespace Persistify.ExternalDtos.Response.Type;

public class ListTypesSuccessResponseDto
{
    public TypeDefinitionDto[] TypeDefinitions { get; init; } = default!;
    public PaginationResponseDto PaginationResponse { get; init; }
}