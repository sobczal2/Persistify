using System.Threading;
using System.Threading.Tasks;
using OneOf;
using Persistify.Dtos.Common.Pagination;
using Persistify.Dtos.Internal.Types;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.StoreManipulation.Types;
using Persistify.Stores.Common;
using Persistify.Stores.Types;

namespace Persistify.RequestHandlers.StoreManipulation.Types;

public class GetPagedTypesFromStoreRequestHandler : StoreManipulationRequestHandler<GetPagedTypesFromStoreRequest, PaginationRequestDto, PagedTypes>
{
    private readonly ITypeStore _typeStore;

    public GetPagedTypesFromStoreRequestHandler(
        ITypeStore typeStore
        )
    {
        _typeStore = typeStore;
    }
    public override async ValueTask<OneOf<StoreSuccess<PagedTypes>, StoreError>> Handle(GetPagedTypesFromStoreRequest request, CancellationToken cancellationToken)
    {
        return await _typeStore.GetPagedAsync(request.Dto, cancellationToken);
    }
}