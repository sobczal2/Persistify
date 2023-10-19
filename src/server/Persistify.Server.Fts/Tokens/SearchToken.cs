using System.Collections.Generic;

namespace Persistify.Server.Fts.Tokens;

public class SearchToken : Token
{
    private SortedSet<int> Positions { get; }
    public SearchToken(string value, char[] alphabet, int position) : base(value, alphabet)
    {
        Positions = new SortedSet<int> { position };
    }
}
