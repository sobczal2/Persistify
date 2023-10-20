using System;
using System.Collections.Generic;
using System.Linq;

namespace Persistify.Server.Fts.Tokens;

public class IndexToken : Token
{
    public SortedSet<DocumentPosition> DocumentPositions { get; set; }

    public IndexToken(string term, char[] alphabet, DocumentPosition documentPosition) : this(term, alphabet, new[] { documentPosition })
    {
    }

    public IndexToken(string term, char[] alphabet, IEnumerable<DocumentPosition> documentPositions) : base(term, alphabet)
    {
        DocumentPositions = new SortedSet<DocumentPosition>(documentPositions);
    }
}
