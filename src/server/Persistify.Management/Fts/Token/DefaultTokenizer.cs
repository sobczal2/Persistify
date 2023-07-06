using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Persistify.Management.Fts.Token;

public class DefaultTokenizer : ITokenizer
{
    public (string Term, float TermFrequency)[] Tokenize(string value)
    {
        var tokens = new Dictionary<string, uint>();
        var valueSpan = value.AsSpan();
        var token = new StringBuilder();

        for (var i = 0; i < valueSpan.Length; i++)
        {
            if (!char.IsLetterOrDigit(valueSpan[i]))
            {
                continue;
            }

            token.Clear();
            var j = i;

            while (j < valueSpan.Length && char.IsLetterOrDigit(valueSpan[j]))
            {
                token.Append(valueSpan[j]);
                j++;
            }

            if (tokens.TryGetValue(token.ToString(), out var termFrequency))
            {
                tokens[token.ToString()] = termFrequency + 1;
            }
            else
            {
                tokens.Add(token.ToString(), 1);
            }

            i = j;
        }

        return tokens.Select(x => (x.Key, (float)x.Value / tokens.Count)).ToArray();
    }

    string[] ITokenizer.TokenizeWithWildcards(string value)
    {
        var tokens = new HashSet<string>();
        var valueSpan = value.AsSpan();
        var token = new StringBuilder();

        for (var i = 0; i < valueSpan.Length; i++)
        {
            if (char.IsLetterOrDigit(valueSpan[i]) || valueSpan[i] == '*' || valueSpan[i] == '?')
            {
                token.Clear();
                var j = i;

                while (j < valueSpan.Length &&
                       (char.IsLetterOrDigit(valueSpan[j]) || valueSpan[j] == '*' || valueSpan[j] == '?'))
                {
                    token.Append(valueSpan[j]);
                    j++;
                }

                tokens.Add(token.ToString());
                i = j;
            }
        }

        return tokens.ToArray();
    }
}
