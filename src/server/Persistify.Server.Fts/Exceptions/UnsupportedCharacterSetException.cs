using System;
using System.Collections.Generic;

namespace Persistify.Server.Fts.Exceptions;

public class UnsupportedCharacterSetException : Exception
{
    public UnsupportedCharacterSetException(string characterSetName,
        IEnumerable<string> supportedCharacterSetNames)
        : base(
            $"Unsupported character set: {characterSetName}. Supported character sets: {string.Join(", ", supportedCharacterSetNames)}")
    {
    }
}
