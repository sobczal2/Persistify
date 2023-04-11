using Persistify.Dtos.Common.Pagination;
using Persistify.Dtos.Internal.Types;
using Persistify.Requests.Common;
using Persistify.Stores.Common;

namespace Persistify.Requests.StoreManipulation.Types;

public class GetPagedTypesFromStoreRequest : StoreManipulationRequest<PaginationRequestDto, PagedTypes>
{
    public GetPagedTypesFromStoreRequest(PaginationRequestDto dto) : base(dto)
    {
    }
}