using System.Threading;
using System.Threading.Tasks;

namespace Persistify.Storage;

public interface IPersisted
{
    ValueTask LoadAsync(IStorage storage, CancellationToken cancellationToken = default);
    ValueTask SaveAsync(IStorage storage, CancellationToken cancellationToken = default);
}