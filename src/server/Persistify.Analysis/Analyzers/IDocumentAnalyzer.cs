using System.Collections.Generic;
using Persistify.Analysis.CharacterFilters;
using Persistify.Analysis.Common;
using Persistify.Analysis.TokenFilters;

namespace Persistify.Analysis.Analyzers;

public interface IDocumentAnalyzer
{
    List<Token> Analyze(string text, ICharacterFilter characterFilter, ITokenFilter tokenFilter);
}
