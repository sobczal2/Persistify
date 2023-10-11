using System;
using System.Collections;
using System.Collections.Generic;
using Persistify.Domain.Fts;

namespace Persistify.Server.Indexes.Indexers.Text;

public class TextIndexerFixedTrieRecord : IComparable<TextIndexerFixedTrieRecord>, IEnumerable<int>
{
    public int DocumentId { get; set; }
    public Token Token { get; set; }

    public TextIndexerFixedTrieRecord(int documentId, Token token)
    {
        DocumentId = documentId;
        Token = token;
    }

    public int CompareTo(TextIndexerFixedTrieRecord? other)
    {
        if (other == null)
        {
            return 1;
        }

        return DocumentId.CompareTo(other.DocumentId);
    }

    public IEnumerator<int> GetEnumerator()
    {
        return Token.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)this).GetEnumerator();
    }
}
