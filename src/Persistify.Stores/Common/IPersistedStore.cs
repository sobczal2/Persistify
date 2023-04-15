using System.Threading;
using System.Threading.Tasks;
using Persistify.Storage;

namespace Persistify.Stores.Common;

public interface IPersistedStore
{
    ValueTask LoadAsync(IStorage storage, CancellationToken cancellationToken = default);
    ValueTask SaveAsync(IStorage storage, CancellationToken cancellationToken = default);
}