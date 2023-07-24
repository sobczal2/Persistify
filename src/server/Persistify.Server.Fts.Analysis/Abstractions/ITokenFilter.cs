using System.Collections.Generic;
using Persistify.Domain.Fts;

namespace Persistify.Server.Fts.Analysis.Abstractions;

public interface ITokenFilter
{
    List<Token> Filter(List<Token> tokens);
}
