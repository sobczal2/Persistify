using System.Threading;
using System.Threading.Tasks;

namespace Persistify.Storage;

public interface IStorage
{
    ValueTask SaveBlobAsync(string key, string data, CancellationToken cancellationToken = default);

    ValueTask<string> LoadBlobAsync(string key, CancellationToken cancellationToken = default);

    ValueTask DeleteBlobAsync(string key, CancellationToken cancellationToken = default);
    ValueTask<bool> ExistsBlobAsync(string key, CancellationToken cancellationToken = default);
}