using System.Collections.Generic;
using Persistify.Server.Fts.Analysis.Tokens;

namespace Persistify.Server.Fts.Analysis.Abstractions;

public interface ITokenizer
{
    List<Token> Tokenize(string text, char[] alphabet);
}
