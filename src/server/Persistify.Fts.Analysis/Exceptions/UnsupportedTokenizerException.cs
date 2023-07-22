using System;
using System.Collections.Generic;

namespace Persistify.Fts.Analysis.Exceptions;

public class UnsupportedTokenizerException : Exception
{
    public UnsupportedTokenizerException(string tokenizerName, IEnumerable<string> supportedTokenizerNames)
        : base(
            $"Unsupported tokenizer: {tokenizerName}. Supported tokenizers: {string.Join(", ", supportedTokenizerNames)}")
    {
    }
}
