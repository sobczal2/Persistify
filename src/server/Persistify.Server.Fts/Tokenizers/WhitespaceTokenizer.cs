using System;
using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.Tokenizers;

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

            tokens.Add(new Token(token.ToString(), index, 1, alphabet));
            index += token.Length + 1;
        }

        return tokens;
    }
}
