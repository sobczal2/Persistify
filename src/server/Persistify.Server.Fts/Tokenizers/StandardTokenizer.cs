using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.Tokenizers;

public class StandardTokenizer : ITokenizer
{
    public List<Token> Tokenize(string text, char[] alphabet)
    {
        var tokens = new Dictionary<string, Token>();
        var charEnumerator = StringInfo.GetTextElementEnumerator(text);

        while (charEnumerator.MoveNext())
        {
            var textElement = charEnumerator.GetTextElement();
            var token = new Token(textElement, charEnumerator.ElementIndex, 1, alphabet);

            if (tokens.TryGetValue(textElement, out var token1))
            {
                token1.Count++;
            }
            else
            {
                tokens.Add(textElement, token);
            }
        }

        return tokens.Values.ToList();
    }
}
