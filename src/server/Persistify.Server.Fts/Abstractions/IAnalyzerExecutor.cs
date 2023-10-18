using System.Collections.Generic;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.Abstractions;

public interface IAnalyzerExecutor
{
    int AlphabetLength { get; }
    IEnumerable<Token> Analyze(string text, AnalyzerMode mode);
}
