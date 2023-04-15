using System;

namespace Persistify.DataStructures.MultiTargetTries.Exceptions;

public class AlphabetTooLargeException : Exception
{
    public AlphabetTooLargeException() : base("Alphabet size is too large")
    {
    }
}