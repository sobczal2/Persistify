using System;
using Persistify.DataStructures.MultiTargetTries;
using Persistify.DataStructures.MultiTargetTries.MultitargetTrieByteTranslationTrie.Mappers;

namespace Persistify.Stores.Objects;

public class TrieIndexMapperFactory
{
    public IMultiTargetMapper CreateMultiTargetMapper(SearchType searchType)
    {
        return searchType switch
        {
            SearchType.CaseSensitive => new StandardCaseSensitiveMultiTargetMapper(),
            // SearchType.CaseInsensitive => new CaseInsensitiveMultiTargetMapper(),
            _ => throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null)
        };
    }
    
    public ISingleTargetMapper CreateSingleTargetMapper(SearchType searchType)
    {
        return searchType switch
        {
            SearchType.CaseSensitive => new StandardCaseSensitiveSingleTargetMapper(),
            // SearchType.CaseInsensitive => new CaseInsensitiveSingleTargetMapper(),
            _ => throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null)
        };
    }
}