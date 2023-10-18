using System;
using System.Collections.Generic;

namespace Persistify.Server.Fts.Exceptions;

public class UnsupportedTokenFilterException : Exception
{
    public UnsupportedTokenFilterException(string tokenFilterName, IEnumerable<string> supportedTokenFilterNames)
        : base(
            $"Unsupported token filter: {tokenFilterName}. Supported token filters: {string.Join(", ", supportedTokenFilterNames)}")
    {
    }
}
