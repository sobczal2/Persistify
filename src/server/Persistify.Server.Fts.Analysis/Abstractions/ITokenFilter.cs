using System.Collections.Generic;
using Persistify.Domain.Fts;

namespace Persistify.Server.Fts.Analysis.Abstractions;

public interface ITokenFilter
{
    void Filter(List<Token> tokens);
    TokenFilterType Type { get; }
}
