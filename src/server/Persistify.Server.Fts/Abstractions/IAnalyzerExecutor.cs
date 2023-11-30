using System.Collections.Generic;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.Abstractions;

public interface IAnalyzerExecutor
{
    int AlphabetLength { get; }

    IEnumerable<SearchToken> AnalyzeForSearch(
        string input
    );

    IEnumerable<IndexToken> AnalyzeForIndex(
        string input,
        int documentId
    );
}
