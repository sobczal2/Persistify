using Persistify.Dtos.Common.Types;
using Persistify.Requests.Common;

namespace Persistify.Requests.StoreManipulation.Types;

public class GetTypeFromStoreRequest : StoreManipulationRequest<string, TypeDefinitionDto>
{
    public GetTypeFromStoreRequest(string dto) : base(dto)
    {
    }
}