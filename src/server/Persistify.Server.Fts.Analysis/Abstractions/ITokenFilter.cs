using System.Collections.Generic;
using Persistify.Domain.Fts;

namespace Persistify.Server.Fts.Analysis.Abstractions;

public interface ITokenFilter
{
    IEnumerable<Token> Filter(IEnumerable<Token> tokens);
    TokenFilterType Type { get; }
}
