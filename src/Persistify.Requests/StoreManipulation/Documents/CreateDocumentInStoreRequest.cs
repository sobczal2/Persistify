using Persistify.Requests.Common;

namespace Persistify.Requests.StoreManipulation.Documents;

public class CreateDocumentInStoreRequest : StoreManipulationRequest<string, long>
{
    public CreateDocumentInStoreRequest(string dto) : base(dto)
    {
    }
}