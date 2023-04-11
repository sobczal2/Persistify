using Persistify.Dtos.Common;
using Persistify.Dtos.Common.Types;
using Persistify.Requests.Common;

namespace Persistify.Requests.StoreManipulation.Types;

public class CreateTypeInStoreRequest : StoreManipulationRequest<TypeDefinitionDto, EmptyDto>
{
    public CreateTypeInStoreRequest(TypeDefinitionDto dto) : base(dto)
    {
    }
}