using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Common.Lifecycle;
using Persistify.Storage;

namespace Persistify.Indexer.Types;

public class InMemoryTypeManager : ITypeManager, IPersistedService
{
    private readonly ILogger<InMemoryTypeManager> _logger;
    public ConcurrentDictionary<string, TypeDefinition> TypeDefinitions { get; set; }

    public InMemoryTypeManager(
        ConcurrentDictionary<string, TypeDefinition> typeDefinitions,
        ILogger<InMemoryTypeManager> logger
        )
    {
        _logger = logger;
        TypeDefinitions = typeDefinitions;
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
}