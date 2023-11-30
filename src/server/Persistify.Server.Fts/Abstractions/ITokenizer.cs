using System.Collections.Generic;
using Persistify.Server.Fts.Tokens;

namespace Persistify.Server.Fts.Abstractions;

public interface ITokenizer
{
    string Code { get; }

    IEnumerable<SearchToken> TokenizeForSearch(
        string text,
        char[] alphabet
    );

    IEnumerable<IndexToken> TokenizeForIndex(
        string text,
        char[] alphabet,
        int documentId
    );
}
