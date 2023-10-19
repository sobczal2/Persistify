using System.Collections.Generic;

namespace Persistify.Server.Fts.Tokens;

public class IndexToken : Token
{
    public SortedSet<DocumentPosition> DocumentPositions { get; set; }

    public IndexToken(string term, char[] alphabet, DocumentPosition documentPosition) : base(term, alphabet)
    {
        DocumentPositions = new SortedSet<DocumentPosition> { documentPosition };
    }
}
