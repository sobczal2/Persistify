using System;
using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.Tokenizers;

public class WhitespaceTokenizer : ITokenizer
{
    private static readonly char[] WhitespaceChars = { ' ', '\t', '\n', '\r' };

    public IEnumerable<SearchToken> TokenizeForSearch(string text, char[] alphabet)
    {
        var index = 0;

        while (index < text.Length)
        {
            var whitespaceIndex = text[index..].IndexOfAny(WhitespaceChars);
            var token = whitespaceIndex == -1
                ? text[index..]
                : text.Substring(index, whitespaceIndex);

            yield return new SearchToken(token, alphabet, index);
            index += token.Length + 1;
        }
    }

    public IEnumerable<IndexToken> TokenizeForIndex(string text, char[] alphabet, int documentId)
    {
        var index = 0;

        while (index < text.Length)
        {
            var whitespaceIndex = text[index..].IndexOfAny(WhitespaceChars);
            var token = whitespaceIndex == -1
                ? text[index..]
                : text.Substring(index, whitespaceIndex);

            yield return new IndexToken(token, alphabet, new DocumentPosition(documentId, index, 1));
            index += token.Length + 1;
        }
    }
}
