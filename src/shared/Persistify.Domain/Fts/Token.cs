using System.Collections.Generic;

namespace Persistify.Domain.Fts;

public class Token
{
    public Token(string value, int count, List<int> positions)
    {
        Value = value;
        Count = count;
        Positions = positions;
    }

    public Token(string value, int position)
    {
        Value = value;
        Count = 1;
        Positions = new List<int>(1) { position };
    }

    public string Value { get; set; }
    public int Count { get; set; }
    public List<int> Positions { get; set; }
}
