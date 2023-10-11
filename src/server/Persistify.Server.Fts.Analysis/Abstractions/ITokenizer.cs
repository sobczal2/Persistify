using System.Collections.Generic;
using Persistify.Domain.Fts;

namespace Persistify.Server.Fts.Analysis.Abstractions;

public interface ITokenizer
{
    List<Token> Tokenize(string text, char[] alphabet);
}
