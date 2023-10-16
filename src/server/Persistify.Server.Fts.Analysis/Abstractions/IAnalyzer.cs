using System.Collections.Generic;
using Persistify.Domain.Fts;
using Persistify.Server.Fts.Analysis.Analyzers;

namespace Persistify.Server.Fts.Analysis.Abstractions;

public interface IAnalyzer
{
    IEnumerable<Token> Analyze(string text, AnalyzerMode mode);
    int AlphabetLength { get; }
}
