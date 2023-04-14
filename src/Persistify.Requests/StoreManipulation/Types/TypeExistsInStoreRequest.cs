using Persistify.Requests.Common;

namespace Persistify.Requests.StoreManipulation.Types;

public class TypeExistsInStoreRequest : StoreManipulationRequest<string, bool>
{
    public TypeExistsInStoreRequest(string dto) : base(dto)
    {
    }
}