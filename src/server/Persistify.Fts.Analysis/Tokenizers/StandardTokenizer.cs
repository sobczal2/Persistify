using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Persistify.Domain.Fts;
using Persistify.Fts.Analysis.Abstractions;

namespace Persistify.Fts.Analysis.Tokenizers;

public class StandardTokenizer : ITokenizer
{
    public List<Token> Tokenize(string text)
    {
        var tokens = new Dictionary<string, Token>();
        var charEnumerator = StringInfo.GetTextElementEnumerator(text);

        while (charEnumerator.MoveNext())
        {
            var textElement = charEnumerator.GetTextElement();
            var token = new Token(textElement, charEnumerator.ElementIndex);

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
