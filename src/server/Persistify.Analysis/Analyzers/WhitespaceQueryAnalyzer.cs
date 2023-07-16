using System;
using Persistify.Analysis.CharacterFilters;

namespace Persistify.Analysis.Analyzers;

public class WhitespaceQueryAnalyzer : IQueryAnalyzer
{
    public string[] Analyze(string text, ICharacterFilter characterFilter)
    {
        return characterFilter.Filter(text).Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }
}
