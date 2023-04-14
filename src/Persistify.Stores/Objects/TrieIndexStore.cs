using System.Collections.Concurrent;
using Persistify.DataStructures.MultiTargetTries;
using Persistify.DataStructures.MultiTargetTries.MultitargetTrieByteTranslationTrie;
using Persistify.Dtos.Common;
using Persistify.Stores.Common;
using OneOf;

namespace Persistify.Stores.Objects;

public class TrieIndexStore : IIndexStore
{
    private readonly ISingleTargetMapper _singleTargetMapper;
    private readonly ConcurrentDictionary<string, MultiTargetTrie<long>> _tries;
    public TrieIndexStore(ISingleTargetMapper singleTargetMapper)
    {
        _singleTargetMapper = singleTargetMapper;
        _tries = new ConcurrentDictionary<string, MultiTargetTrie<long>>();
    }
    public OneOf<StoreSuccess<EmptyDto>, StoreError> AddTokens(string typeName, string[] tokens, long documentId)
    {
        if (!_tries.TryGetValue(typeName, out var trie))
        {
            trie = new MultiTargetTrie<long>(_singleTargetMapper.AlphabetSize);
            _tries.TryAdd(typeName, trie);
        }
        foreach (var token in tokens)
        {
            trie.Add(token, documentId, _singleTargetMapper);
        }
        
        return new StoreSuccess<EmptyDto>(new EmptyDto());
    }
}