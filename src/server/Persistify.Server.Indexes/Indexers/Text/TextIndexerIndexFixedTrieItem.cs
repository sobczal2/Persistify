using System;
using System.Collections;
using System.Collections.Generic;
using Persistify.Server.Fts.Tokens;
using Persistify.Server.Indexes.DataStructures.Tries;

namespace Persistify.Server.Indexes.Indexers.Text;

public class TextIndexerIndexFixedTrieItem : IndexFixedTrieItem<TextIndexerIndexFixedTrieItem>
{
    public TextIndexerIndexFixedTrieItem(int documentId, Token token)
    {
        DocumentId = documentId;
        Token = token;
    }

    public int DocumentId { get; set; }
    public Token Token { get; set; }

    public override int GetIndex(int index)
    {
        return Token.AlphabetIndexMap[index];
    }

    public override int Length => Token.Value.Length;
    public override int CompareTo(TextIndexerIndexFixedTrieItem? other)
    {
        if (other == null)
        {
            return 1;
        }

        return DocumentId.CompareTo(other.DocumentId);
    }

    public override TextIndexerIndexFixedTrieItem Value => this;
}
