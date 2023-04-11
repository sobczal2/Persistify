using System.Threading;
using System.Threading.Tasks;
using Mediator;
using OneOf;
using Persistify.Requests.Common;
using Persistify.Stores.Common;

namespace Persistify.RequestHandlers.Common;

public abstract class
    StoreManipulationRequestHandler<TRequest, TDto, TStoreResult> : IRequestHandler<TRequest,
        OneOf<StoreSuccess<TStoreResult>, StoreError>>
    where TRequest : StoreManipulationRequest<TDto, TStoreResult>
    where TDto : notnull
    where TStoreResult : notnull
{
    public abstract ValueTask<OneOf<StoreSuccess<TStoreResult>, StoreError>> Handle(TRequest request,
        CancellationToken cancellationToken);
}