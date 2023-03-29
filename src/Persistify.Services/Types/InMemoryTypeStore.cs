using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Common.Lifecycle;
using Persistify.Storage;

namespace Persistify.Indexer.Types;

public class InMemoryTypeStore : ITypeStore, IPersistedService
{
    private readonly ILogger<InMemoryTypeStore> _logger;

    public InMemoryTypeStore(ILogger<InMemoryTypeStore> logger)
    {
        _logger = logger;
        TypeDefinitions = new ConcurrentDictionary<string, TypeDefinition>();
    }

    public ConcurrentDictionary<string, TypeDefinition> TypeDefinitions { get; set; }

    public Task LoadAsync(IStorageProvider storageProvider)
    {
        _logger.LogInformation("Loading types from storage provider");
        return Task.CompletedTask;
    }

    public Task SaveAsync(IStorageProvider storageProvider)
    {
        _logger.LogInformation("Saving types to storage provider");
        return Task.CompletedTask;
    }

    public Task<bool> InitTypeAsync(TypeDefinition typeDefinition)
    {
        return Task.FromResult(TypeDefinitions.TryAdd(typeDefinition.Name, typeDefinition));
    }

    public Task<TypeDefinition[]> ListTypesAsync()
    {
        return Task.FromResult(TypeDefinitions.Values.ToArray());
    }

    public Task<bool> DropTypeAsync(string typeName)
    {
        return Task.FromResult(TypeDefinitions.TryRemove(typeName, out _));
    }
}