using Persistify.Dtos.Common.Pagination;
using Persistify.Dtos.Common.Types;

namespace Persistify.Dtos.Internal.Types;

public class PagedTypes
{
    public PagedTypes(TypeDefinitionDto[] types, PaginationResponseDto paginationResponse)
    {
        Types = types;
        PaginationResponse = paginationResponse;
    }

    public TypeDefinitionDto[] Types { get; }
    public PaginationResponseDto PaginationResponse { get; }
}