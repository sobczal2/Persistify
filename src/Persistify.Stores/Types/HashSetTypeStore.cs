using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Persistify.Helpers;
using Persistify.Protos;
using Persistify.Storage;
using Persistify.Stores.Common;

namespace Persistify.Stores.Types;

public class HashSetTypeStore : ITypeStore, IPersisted
{
    private ConcurrentDictionary<string, TypeDefinitionProto>? _types;

    public async ValueTask LoadAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        var exists = await storage.ExistsBlobAsync("types", cancellationToken);
        if (exists)
        {
            var result = await storage.LoadBlobAsync("types", cancellationToken);
            _types = JsonConvert.DeserializeObject<ConcurrentDictionary<string, TypeDefinitionProto>>(result);
            return;
        }

        _types = new ConcurrentDictionary<string, TypeDefinitionProto>();
    }

    public async ValueTask SaveAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        AssertInitialized();
        await storage.SaveBlobAsync("types", JsonConvert.SerializeObject(_types), cancellationToken);
    }

    public bool Exists(string type)
    {
        AssertInitialized();
        return _types!.ContainsKey(type);
    }

    public void Create(TypeDefinitionProto type)
    {
        AssertInitialized();
        if (!_types!.TryAdd(type.Name, type))
            throw new StoreException("Type not found");
    }

    public void Delete(string typeName)
    {
        AssertInitialized();
        if (!_types!.TryRemove(typeName, out _))
            throw new StoreException("Type not found");
    }

    public TypeDefinitionProto Get(string typeName)
    {
        AssertInitialized();
        if (!_types!.TryGetValue(typeName, out var typeDefinition))
            throw new StoreException("Type not found");
        return typeDefinition;
    }

    public (TypeDefinitionProto[] TypeDefinitions, PaginationResponseProto PaginationResponse) List(
        PaginationRequestProto paginationRequest)
    {
        AssertInitialized();
        var typeDefinitions = _types!.Values.ToList();
        var totalCount = typeDefinitions.Count;
        return (
            typeDefinitions
                .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
                .Take(paginationRequest.PageSize)
                .ToArray(),
            new PaginationResponseProto
            {
                PageNumber = paginationRequest.PageNumber,
                PageSize = paginationRequest.PageSize,
                TotalItems = totalCount,
                TotalPages = MathI.Ceiling(totalCount / (double)paginationRequest.PageSize)
            });
    }

    private void AssertInitialized()
    {
        if (_types == null)
            throw new StoreException("Store not yet initialized");
    }
}