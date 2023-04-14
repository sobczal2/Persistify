using System.Threading;
using System.Threading.Tasks;
using OneOf;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.StoreManipulation.Types;
using Persistify.Stores.Common;
using Persistify.Stores.Types;

namespace Persistify.RequestHandlers.StoreManipulation.Types;

public class TypeExistsInStoreRequestHandler : StoreManipulationRequestHandler<TypeExistsInStoreRequest, string, bool>
{
    private readonly ITypeStore _typeStore;

    public TypeExistsInStoreRequestHandler(ITypeStore typeStore)
    {
        _typeStore = typeStore;
    }
    public override async  ValueTask<OneOf<StoreSuccess<bool>, StoreError>> Handle(TypeExistsInStoreRequest request, CancellationToken cancellationToken)
    {
        return await _typeStore.ExistsAsync(request.Dto, cancellationToken);
    }
}