using System;
using System.Collections.Generic;
using System.Linq;
using Persistify.Analysis.CharacterFilters;
using Persistify.Analysis.Common;
using Persistify.Analysis.TokenFilters;

namespace Persistify.Analysis.Analyzers;

public class WhitespaceDocumentAnalyzer : IDocumentAnalyzer
{
    public List<Token> Analyze(string text, ICharacterFilter characterFilter, ITokenFilter tokenFilter)
    {
        var filteredText = characterFilter.Filter(text);

        var textSpan = text.AsSpan();

        var tokens = new Dictionary<string, Token>();

        var tokenStart = 0;
        var tokenLength = 0;

        for (var i = 0; i < filteredText.Length; i++)
        {
            var character = filteredText[i];
            if (char.IsWhiteSpace(character))
            {
                if (tokenLength > 0)
                {
                    var token = textSpan.Slice(tokenStart, tokenLength).ToString();
                    if (!tokens.ContainsKey(token))
                    {
                        tokens.Add(token, new Token(token, 1, new List<int> { tokenStart }));
                    }
                    else
                    {
                        tokens[token].Count++;
                        tokens[token].Positions.Add(tokenStart);
                    }
                }

                tokenStart = i + 1;
                tokenLength = 0;
            }
            else
            {
                tokenLength++;
            }
        }

        return tokenFilter.Filter(tokens.Values.ToList());
    }
}
