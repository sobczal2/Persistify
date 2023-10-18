using System.Collections.Generic;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.Abstractions;

public interface ITokenFilter
{
    TokenFilterType Type { get; }
    void Filter(List<Token> tokens);
}
