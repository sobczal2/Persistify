using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Persistify.Indexes.Common;
using Persistify.Storage;
using Persistify.Tokens;

namespace Persistify.Indexes.Boolean;

public class BooleanIndexer : IIndexer<bool>, IPersisted
{
    private const string IndexerName = "booleanindexer";
    private const string TrueSetsName = "truesets";
    private const string FalseSetsName = "falsesets";
    private ConcurrentDictionary<TypePath, HashSet<long>> _falseSets = default!;
    private ConcurrentDictionary<TypePath, HashSet<long>> _trueSets = default!;

    public Task IndexAsync(long id, Token<bool> token, string typeName)
    {
        AssertInitialized();
        var sets = token.Value ? _trueSets : _falseSets;
        var set = sets.GetOrAdd(new TypePath(typeName, token.Path), _ => new HashSet<long>());
        set.Add(id);
        return Task.CompletedTask;
    }

    public Task IndexAsync(long id, IEnumerable<Token<bool>> tokens, string typeName)
    {
        AssertInitialized();
        var groupedTokens = tokens.GroupBy(token => token.Path);
        foreach (var tokenGroup in groupedTokens)
        {
            var trueSet = _trueSets.GetOrAdd(new TypePath(typeName, tokenGroup.Key), _ => new HashSet<long>());
            var falseSet = _falseSets.GetOrAdd(new TypePath(typeName, tokenGroup.Key), _ => new HashSet<long>());
            foreach (var token in tokenGroup)
                if (token.Value)
                    trueSet.Add(id);
                else
                    falseSet.Add(id);
        }

        return Task.CompletedTask;
    }

    public Task<IEnumerable<long>> SearchAsync(ISearchPredicate searchPredicate)
    {
        AssertInitialized();
        if (searchPredicate is not BooleanSearchPredicate booleanSearchPredicate)
            throw new ArgumentException("Search predicate must be of type BooleanSearchPredicate");

        var sets = booleanSearchPredicate.Value ? _trueSets : _falseSets;

        var set = sets.GetOrAdd(
            new TypePath(searchPredicate.TypeName, searchPredicate.Path),
            _ => new HashSet<long>());
        return Task.FromResult<IEnumerable<long>>(set);
    }

    public Task<int> RemoveAsync(long id, string typeName)
    {
        AssertInitialized();
        var trueSets = _trueSets.Where(pair => pair.Key.TypeName == typeName);
        var falseSets = _falseSets.Where(pair => pair.Key.TypeName == typeName);
        var count = trueSets.Sum(set => set.Value.Remove(id) ? 1 : 0);
        count += falseSets.Sum(set => set.Value.Remove(id) ? 1 : 0);

        return Task.FromResult(count);
    }

    public async ValueTask LoadAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        var trueExists = await storage.ExistsBlobAsync($"{IndexerName}_{TrueSetsName}", cancellationToken);
        var falseExists = await storage.ExistsBlobAsync($"{IndexerName}_{FalseSetsName}", cancellationToken);
        var exists = trueExists && falseExists;
        var trueSets = new ConcurrentDictionary<TypePath, HashSet<long>>();
        var falseSets = new ConcurrentDictionary<TypePath, HashSet<long>>();

        if (!exists)
        {
            _trueSets = trueSets;
            _falseSets = falseSets;
            return;
        }

        var trueSerializedSets = await storage.LoadBlobAsync($"{IndexerName}_{TrueSetsName}", cancellationToken);
        var falseSerializedSets = await storage.LoadBlobAsync($"{IndexerName}_{FalseSetsName}", cancellationToken);

        trueSets = JsonConvert.DeserializeObject<ConcurrentDictionary<TypePath, HashSet<long>>>(
            trueSerializedSets) ?? throw new InvalidOperationException("Could not deserialize sets");

        falseSets = JsonConvert.DeserializeObject<ConcurrentDictionary<TypePath, HashSet<long>>>(
            falseSerializedSets) ?? throw new InvalidOperationException("Could not deserialize sets");

        _trueSets = trueSets;

        _falseSets = falseSets;
    }


    public async ValueTask SaveAsync(IStorage storage, CancellationToken cancellationToken = default)
    {
        AssertInitialized();

        string serializedTrueSets;
        string serializedFalseSets;

        lock (_trueSets)
        {
            serializedTrueSets = JsonConvert.SerializeObject(_trueSets);
        }

        lock (_falseSets)
        {
            serializedFalseSets = JsonConvert.SerializeObject(_falseSets);
        }

        await storage.SaveBlobAsync($"{IndexerName}_{TrueSetsName}", serializedTrueSets, cancellationToken);
        await storage.SaveBlobAsync($"{IndexerName}_{FalseSetsName}", serializedFalseSets, cancellationToken);
    }


    private void AssertInitialized()
    {
        if (_trueSets == null || _falseSets == null)
            throw new InvalidOperationException("Indexer has not been initialized");
    }
}