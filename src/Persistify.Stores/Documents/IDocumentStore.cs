using System.Threading;
using System.Threading.Tasks;
using OneOf;
using Persistify.Dtos.Common;
using Persistify.Stores.Common;

namespace Persistify.Stores.Documents;

public interface IDocumentStore
{
    ValueTask<OneOf<StoreSuccess<string>, StoreError>> GetAsync(long documentId, CancellationToken cancellationToken = default);
    ValueTask<OneOf<StoreSuccess<long>, StoreError>> AddAsync(string document, CancellationToken cancellationToken = default);
    ValueTask<OneOf<StoreSuccess<EmptyDto>, StoreError>> RemoveAsync(long documentId, CancellationToken cancellationToken = default);
    ValueTask<OneOf<StoreSuccess<bool>, StoreError>> ExistsAsync(long documentId, CancellationToken cancellationToken = default);

}