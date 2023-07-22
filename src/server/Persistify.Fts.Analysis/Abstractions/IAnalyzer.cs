using System.Collections.Generic;
using Persistify.Domain.Fts;

namespace Persistify.Fts.Analysis.Abstractions;

public interface IAnalyzer
{
    List<Token> Analyze(string text);
}
