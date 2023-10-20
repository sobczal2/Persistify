using System;
using Persistify.Server.Fts.Tokens;
using Persistify.Server.Indexes.DataStructures.Tries;

namespace Persistify.Server.Indexes.Indexers.Text;

public class TextIndexerSearchFixedTrieItem : SearchFixedTrieItem
{
    private readonly int[] _alphabetIndexMap;
    private readonly Token _token;

    public TextIndexerSearchFixedTrieItem(
        Token token
    )
    {
        _token = token;

        _alphabetIndexMap = new int[_token.Term.Length];

        for (var i = 0; i < _token.Term.Length; i++)
        {
            _alphabetIndexMap[i] = token.Term[i] switch
            {
                '?' => AnyIndex,
                '*' => RepeatedAnyIndex,
                var term => Array.BinarySearch(_token.Alphabet, term)
            };
        }
    }

    public override int Length => _token.Term.Length;
    public sealed override int AnyIndex => _alphabetIndexMap.Length;
    public sealed override int RepeatedAnyIndex => _alphabetIndexMap.Length + 1;

    public override int GetIndex(int index)
    {
        if (index >= _alphabetIndexMap.Length)
        {
            return -1;
        }

        if (_alphabetIndexMap[index] < 0)
        {
            return -1;
        }

        return _alphabetIndexMap[index];
    }
}
