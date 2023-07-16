using System.Collections.Generic;
using Persistify.Analysis.Common;

namespace Persistify.Analysis.TokenFilters;

public class PassthroughTokenFilter : ITokenFilter
{
    public List<Token> Filter(List<Token> tokens) => tokens;
}
