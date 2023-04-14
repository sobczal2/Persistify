using Persistify.Dtos.Common;
using Persistify.Requests.Common;

namespace Persistify.Requests.StoreManipulation.Indexes;

public class AddTokensInStoreRequest : StoreManipulationRequest<(string TypeName, string[] Tokens, long DocumentId), EmptyDto>
{
    public AddTokensInStoreRequest((string TypeName, string[] Tokens, long DocumentId) dto) : base(dto)
    {
    }
}