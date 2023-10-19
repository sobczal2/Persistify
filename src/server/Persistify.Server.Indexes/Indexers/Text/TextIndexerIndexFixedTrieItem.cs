using System;
using System.Collections;
using System.Collections.Generic;
using Persistify.Server.Fts.Tokens;
using Persistify.Server.Indexes.DataStructures.Tries;

namespace Persistify.Server.Indexes.Indexers.Text;

public class TextIndexerIndexFixedTrieItem : IndexFixedTrieItem<IndexToken>
{
    public TextIndexerIndexFixedTrieItem(
        IndexToken value
        )
    {
        Value = value;
    }
    public override int GetIndex(int index)
    {
        return Array.BinarySearch(Value.Alphabet, Value.Term[index]);
    }

    public override int Length => Value.Term.Length;
    public override IndexToken Value { get; }

    public override void Merge(IndexToken other)
    {
        Value.DocumentPositions.UnionWith(other.DocumentPositions);
    }

    public override bool IsEmpty => Value.DocumentPositions.Count == 0;
}
