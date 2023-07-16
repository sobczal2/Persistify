using System.Collections.Generic;
using Persistify.Analysis.Common;

namespace Persistify.Analysis.TokenFilters;

public interface ITokenFilter
{
    List<Token> Filter(List<Token> tokens);
}
