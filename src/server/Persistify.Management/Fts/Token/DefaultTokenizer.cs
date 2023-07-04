using System;
using System.Collections.Generic;
using System.Text;

namespace Persistify.Management.Fts.Token;

public class DefaultTokenizer : ITokenizer
{
    public ISet<string> Tokenize(string value)
    {
        var tokens = new HashSet<string>();
        var valueSpan = value.AsSpan();
        var token = new StringBuilder();
        
        for (var i = 0; i < valueSpan.Length; i++)
        {
            if (char.IsLetterOrDigit(valueSpan[i]))
            {
                token.Clear();
                var j = i;
                
                while (j < valueSpan.Length && char.IsLetterOrDigit(valueSpan[j]))
                {
                    token.Append(valueSpan[j]);
                    j++;
                }
                
                tokens.Add(token.ToString());
                i = j;
            }
        }
        
        return tokens;
    }
    
    public ISet<string> TokenizeWithWildcards(string value)
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
                
                while (j < valueSpan.Length && char.IsLetterOrDigit(valueSpan[j]) || valueSpan[j] == '*' || valueSpan[j] == '?')
                {
                    token.Append(valueSpan[j]);
                    j++;
                }
                
                tokens.Add(token.ToString());
                i = j;
            }
        }
        
        return tokens;
    }
    
}
