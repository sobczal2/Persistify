using System.Threading;
using System.Threading.Tasks;
using OneOf;
using Persistify.Dtos.Common;
using Persistify.Dtos.Common.Types;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.StoreManipulation.Types;
using Persistify.Stores.Common;
using Persistify.Stores.Types;

namespace Persistify.RequestHandlers.StoreManipulation.Types;

public class
    CreateTypeInStoreRequestHandler : StoreManipulationRequestHandler<CreateTypeInStoreRequest, TypeDefinitionDto,
        EmptyDto>
{
    private readonly ITypeStore _typeStore;

    public CreateTypeInStoreRequestHandler(
        ITypeStore typeStore
    )
    {
        _typeStore = typeStore;
    }

    public override async ValueTask<OneOf<StoreSuccess<EmptyDto>, StoreError>> Handle(CreateTypeInStoreRequest request,
        CancellationToken cancellationToken)
    {
        var existingResult = await _typeStore.ExistsAsync(request.Dto.TypeName, cancellationToken);
        if (existingResult.IsT1) return existingResult.AsT1;
        
        if (existingResult.AsT0.Data)
            return new StoreError("Type already exists", StoreErrorType.AlreadyExists);

        return await _typeStore.CreateAsync(request.Dto, cancellationToken);
    }
}