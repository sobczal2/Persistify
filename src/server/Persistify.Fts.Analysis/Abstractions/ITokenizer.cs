using System.Collections.Generic;
using Persistify.Domain.Fts;

namespace Persistify.Fts.Analysis.Abstractions;

public interface ITokenizer
{
    List<Token> Tokenize(string text);
}
