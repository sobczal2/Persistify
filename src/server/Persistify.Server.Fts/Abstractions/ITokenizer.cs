using System.Collections.Generic;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.Abstractions;

public interface ITokenizer
{
    List<Token> Tokenize(string text, char[] alphabet);
}
