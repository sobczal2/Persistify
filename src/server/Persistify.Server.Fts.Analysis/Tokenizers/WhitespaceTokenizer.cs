using System;
using System.Collections.Generic;
using Persistify.Domain.Fts;
using Persistify.Server.Fts.Analysis.Abstractions;

namespace Persistify.Server.Fts.Analysis.Tokenizers;

public class WhitespaceTokenizer : ITokenizer
{
    private static readonly char[] WhitespaceChars = { ' ', '\t', '\n', '\r' };

    public List<Token> Tokenize(string text, char[] alphabet)
    {
        var textSpan = text.AsSpan();
        var tokens = new List<Token>();
        var index = 0;

        while (index < textSpan.Length)
        {
            var whitespaceIndex = textSpan[index..].IndexOfAny(WhitespaceChars);
            var token = whitespaceIndex == -1
                ? textSpan[index..]
                : textSpan.Slice(index, whitespaceIndex);

            tokens.Add(new Token(token.ToString(), index, token.Length, alphabet));
            index += token.Length + 1;
        }

        return tokens;
    }
}
