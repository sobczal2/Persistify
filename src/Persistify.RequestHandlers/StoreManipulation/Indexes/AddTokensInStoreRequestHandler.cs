using System.Threading;
using System.Threading.Tasks;
using OneOf;
using Persistify.Dtos.Common;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.StoreManipulation.Indexes;
using Persistify.Stores.Common;
using Persistify.Stores.Objects;

namespace Persistify.RequestHandlers.StoreManipulation.Indexes;

public class AddTokensInStoreRequestHandler : StoreManipulationRequestHandler<AddTokensInStoreRequest, (string TypeName,
    string[] Tokens, long DocumentId), EmptyDto>
{
    private readonly IIndexStore _indexStore;

    public AddTokensInStoreRequestHandler(IIndexStore indexStore)
    {
        _indexStore = indexStore;
    }

    public override ValueTask<OneOf<StoreSuccess<EmptyDto>, StoreError>> Handle(AddTokensInStoreRequest request,
        CancellationToken cancellationToken)
    {
        var addTokensResponse = _indexStore.AddTokens(request.Dto.TypeName, request.Dto.Tokens, request.Dto.DocumentId);

        return new ValueTask<OneOf<StoreSuccess<EmptyDto>, StoreError>>(addTokensResponse);
    }
}