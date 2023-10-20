using System;
using System.Linq;

namespace Persistify.Server.Fts.Tokens;

public abstract class Token
{
    protected Token(string term, char[] alphabet)
    {
        Alphabet = alphabet;
        Term = term;
    }

    public char[] Alphabet { get; }
    public string Term { get; set; }
}
