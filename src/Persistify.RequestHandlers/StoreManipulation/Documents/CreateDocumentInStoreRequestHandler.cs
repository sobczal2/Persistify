using System.Threading;
using System.Threading.Tasks;
using OneOf;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.StoreManipulation.Documents;
using Persistify.Stores.Common;
using Persistify.Stores.Documents;

namespace Persistify.RequestHandlers.StoreManipulation.Documents;

public class CreateDocumentInStoreRequestHandler : StoreManipulationRequestHandler<CreateDocumentInStoreRequest, string, long>
{
    private readonly IDocumentStore _documentStore;

    public CreateDocumentInStoreRequestHandler(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }
    public override async ValueTask<OneOf<StoreSuccess<long>, StoreError>> Handle(CreateDocumentInStoreRequest request, CancellationToken cancellationToken)
    {
        var documentIdResponse = await _documentStore.AddAsync(request.Dto, cancellationToken);
        
        return documentIdResponse.Match<OneOf<StoreSuccess<long>, StoreError>>(
            success => new StoreSuccess<long>(success.Data),
            error => new StoreError(error.Message, StoreErrorType.Unknown)
        );
    }
}