using System.Collections.Generic;
using Persistify.Server.Fts.Analysis.Tokens;

namespace Persistify.Server.Fts.Analysis.Abstractions;

public interface ITokenFilter
{
    TokenFilterType Type { get; }
    void Filter(List<Token> tokens);
}
