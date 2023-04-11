using System.Threading;
using System.Threading.Tasks;
using OneOf;
using Persistify.Dtos.Common;
using Persistify.Dtos.Common.Pagination;
using Persistify.Dtos.Common.Types;
using Persistify.Dtos.Internal.Types;
using Persistify.Stores.Common;

namespace Persistify.Stores.Types;

public interface ITypeStore
{
    ValueTask<OneOf<StoreSuccess<bool>, StoreError>> ExistsAsync(string type,
        CancellationToken cancellationToken = default);

    ValueTask<OneOf<StoreSuccess<EmptyDto>, StoreError>> CreateAsync(TypeDefinitionDto typeDefinitionDto,
        CancellationToken cancellationToken = default);

    ValueTask<OneOf<StoreSuccess<EmptyDto>, StoreError>> DeleteAsync(string type,
        CancellationToken cancellationToken = default);

    ValueTask<OneOf<StoreSuccess<PagedTypes>, StoreError>> GetPagedAsync(PaginationRequestDto paginationRequest,
        CancellationToken cancellationToken = default);
}