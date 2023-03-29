using System;

namespace Persistify.DataStructures.Tries.Exceptions;

public class InvalidCharsInKeyException : Exception
{
    public InvalidCharsInKeyException(char c) : base($"Invalid character '{c}' in key.")
    {
    }
}