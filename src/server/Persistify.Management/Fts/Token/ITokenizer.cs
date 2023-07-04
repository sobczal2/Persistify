using System.Collections.Generic;

namespace Persistify.Management.Fts.Token;

public interface ITokenizer
{
    ISet<string> Tokenize(string value);
    ISet<string> TokenizeWithWildcards(string value);
}
