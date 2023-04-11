using System.Threading.Tasks;
using OneOf;
using Persistify.Dtos.Common;
using Persistify.Storage;

namespace Persistify.Stores.Common;

public interface IPersistedStore
{
    ValueTask<OneOf<StoreSuccess<EmptyDto>, StoreError>> LoadAsync(IStorage storage);
    ValueTask<OneOf<StoreSuccess<EmptyDto>, StoreError>> SaveAsync(IStorage storage);
}