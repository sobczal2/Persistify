using System.Collections.Generic;
using Persistify.Server.Fts.Analysis.Tokens;

namespace Persistify.Server.Fts.Analysis.Abstractions;

public interface IAnalyzer
{
    int AlphabetLength { get; }
    IEnumerable<Token> Analyze(string text, AnalyzerMode mode);
}
