using System;
using Persistify.Analysis.CharacterFilters;

namespace Persistify.Analysis.Analyzers;

public interface IQueryAnalyzer
{
    string[] Analyze(string text, ICharacterFilter characterFilter);
}
