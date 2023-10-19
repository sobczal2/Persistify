using System;
using System.Linq;

namespace Persistify.Server.Fts.Tokens;

public abstract class Token
{
    protected Token(string term, char[] alphabet)
    {
        Alphabet = alphabet;
        Term = term;

        if (!CheckIfValueIsInAlphabet())
        {
            throw new ArgumentException("Either value is not in given alphabet or alphabet is not sorted",
                nameof(Alphabet));
        }
    }

    public char[] Alphabet { get; }
    public string Term { get; set; }

    private bool CheckIfValueIsInAlphabet()
    {
        return Term.All(x => Array.BinarySearch(Alphabet, x) >= 0);
    }
}
