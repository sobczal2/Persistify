using System.Threading;
using System.Threading.Tasks;

namespace Persistify.Stores.Documents;

public interface IDocumentStore
{
    ValueTask<string> GetAsync(long documentId, CancellationToken cancellationToken = default);
    ValueTask<long> AddAsync(string document, CancellationToken cancellationToken = default);
    ValueTask RemoveAsync(long documentId, CancellationToken cancellationToken = default);
    ValueTask<bool> ExistsAsync(long documentId, CancellationToken cancellationToken = default);
}