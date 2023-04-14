using System.Threading;
using System.Threading.Tasks;
using OneOf;
using Persistify.Dtos.Common.Types;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.StoreManipulation.Types;
using Persistify.Stores.Common;
using Persistify.Stores.Types;

namespace Persistify.RequestHandlers.StoreManipulation.Types;

public class GetTypeFromStoreRequestHandler : StoreManipulationRequestHandler<GetTypeFromStoreRequest, string, TypeDefinitionDto>
{
    private readonly ITypeStore _typeStore;

    public GetTypeFromStoreRequestHandler(ITypeStore typeStore)
    {
        _typeStore = typeStore;
    }
    public override ValueTask<OneOf<StoreSuccess<TypeDefinitionDto>, StoreError>> Handle(GetTypeFromStoreRequest request, CancellationToken cancellationToken)
    {
        return _typeStore.GetAsync(request.Dto, cancellationToken);
    }
}