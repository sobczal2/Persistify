using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OneOf;
using Persistify.Dtos.Common;
using Persistify.Dtos.Common.Pagination;
using Persistify.Dtos.Common.Types;
using Persistify.Dtos.Internal.Types;
using Persistify.Storage;
using Persistify.Stores.Common;

namespace Persistify.Stores.Types;

public class HashSetTypeStore : ITypeStore, IPersistedStore
{
    private ConcurrentDictionary<string, TypeDefinitionDto>? _types;

    public async ValueTask<OneOf<StoreSuccess<EmptyDto>, StoreError>> LoadAsync(IStorage storage)
    {
        var exists = await storage.ExistsBlobAsync("types");
        if (exists.IsT1) return new StoreError(exists.AsT1.Message, StoreErrorType.StorageError);

        if (exists.AsT0.Data)
        {
            var result = await storage.LoadBlobAsync("types");
            return result.Match<OneOf<StoreSuccess<EmptyDto>, StoreError>>(
                success =>
                {
                    _types =
                        JsonConvert.DeserializeObject<ConcurrentDictionary<string, TypeDefinitionDto>>(success.Data);
                    return new StoreSuccess<EmptyDto>(new EmptyDto());
                },
                error => new StoreError(error.Message, StoreErrorType.StorageError)
            );
        }

        _types = new ConcurrentDictionary<string, TypeDefinitionDto>();
        return new StoreSuccess<EmptyDto>(new EmptyDto());
    }

    public async ValueTask<OneOf<StoreSuccess<EmptyDto>, StoreError>> SaveAsync(IStorage storage)
    {
        if(_types == null)
            return new StoreError("Store not yet initialized", StoreErrorType.InvalidState);
        var result = await storage.SaveBlobAsync("types", JsonConvert.SerializeObject(_types), true);
        return result.Match<OneOf<StoreSuccess<EmptyDto>, StoreError>>(
            _ => new StoreSuccess<EmptyDto>(new EmptyDto()),
            error => new StoreError(error.Message, StoreErrorType.StorageError)
        );
    }

    public ValueTask<OneOf<StoreSuccess<bool>, StoreError>> ExistsAsync(string type,
        CancellationToken cancellationToken = default)
    {
        if (_types == null)
            return ValueTask.FromResult<OneOf<StoreSuccess<bool>, StoreError>>(
                new StoreError("Store not yet initialized", StoreErrorType.InvalidState));

        return ValueTask.FromResult<OneOf<StoreSuccess<bool>, StoreError>>(
            new StoreSuccess<bool>(_types.ContainsKey(type)));
    }

    public ValueTask<OneOf<StoreSuccess<EmptyDto>, StoreError>> CreateAsync(TypeDefinitionDto type,
        CancellationToken cancellationToken = default)
    {
        if (_types == null)
            return ValueTask.FromResult<OneOf<StoreSuccess<EmptyDto>, StoreError>>(
                new StoreError("Store not yet initialized", StoreErrorType.InvalidState));

        if (!_types.TryAdd(type.TypeName, type))
            return ValueTask.FromResult<OneOf<StoreSuccess<EmptyDto>, StoreError>>(
                new StoreError("Type already exists", StoreErrorType.AlreadyExists));
        return ValueTask.FromResult<OneOf<StoreSuccess<EmptyDto>, StoreError>>(
            new StoreSuccess<EmptyDto>(new EmptyDto()));
    }

    public ValueTask<OneOf<StoreSuccess<EmptyDto>, StoreError>> DeleteAsync(string type,
        CancellationToken cancellationToken = default)
    {
        if (_types == null)
            return ValueTask.FromResult<OneOf<StoreSuccess<EmptyDto>, StoreError>>(
                new StoreError("Store not yet initialized", StoreErrorType.InvalidState));

        _types.TryRemove(type, out _);
        return ValueTask.FromResult<OneOf<StoreSuccess<EmptyDto>, StoreError>>(
            new StoreSuccess<EmptyDto>(new EmptyDto()));
    }

    public ValueTask<OneOf<StoreSuccess<PagedTypes>, StoreError>> GetPagedAsync(
        PaginationRequestDto paginationRequest, CancellationToken cancellationToken = default)
    {
        if (_types == null)
            return ValueTask.FromResult<OneOf<StoreSuccess<PagedTypes>, StoreError>>(
                new StoreError("Store not yet initialized", StoreErrorType.InvalidState));

        var types = _types.Values.ToArray();
        var skip = (paginationRequest.PageNumber - 1) * paginationRequest.PageSize;

        var pagedTypes = types.Skip(skip).Take(paginationRequest.PageSize).ToArray();
        var paginationResponse = new PaginationResponseDto(paginationRequest.PageNumber, paginationRequest.PageSize,
            types.Length);

        return ValueTask.FromResult<OneOf<StoreSuccess<PagedTypes>, StoreError>>(
            new StoreSuccess<PagedTypes>(new PagedTypes(pagedTypes, paginationResponse)));
    }
}