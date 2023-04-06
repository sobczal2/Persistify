using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Persistify.Indexer.Types;

namespace Persistify.Indexer.Tokens;

public class JsonLowercaseTokenizer : ITokenizer
{
    public Token[] Tokenize(TypeDefinition typeDefinition, string data)
    {
        var jobject = JObject.Parse(data);

        var tokens = new List<Token>(typeDefinition.TypeFields.Length * 2);
        foreach (var typeField in typeDefinition.TypeFields)
        {
            var jtoken = jobject.SelectToken(typeField.Path);
            if (jtoken == null)
            {
                continue;
            }

            var tokenStrings = jtoken.Value<string>().Split(' ');
            foreach (var tokenString in tokenStrings)
            {
                tokens.Add(new Token(tokenString.ToLowerInvariant()));
            }
        }
        
        return tokens.ToArray();
    }
}