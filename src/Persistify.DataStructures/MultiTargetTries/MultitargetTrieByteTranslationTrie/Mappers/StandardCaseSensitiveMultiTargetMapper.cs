using System;
using System.Text.RegularExpressions;

namespace Persistify.DataStructures.MultiTargetTries.MultitargetTrieByteTranslationTrie.Mappers;

public class StandardCaseSensitiveMultiTargetMapper : IMultiTargetMapper
{
    private static readonly byte[] FullAlphabet = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61 };
    public byte[] MapToIndexes(char value)
    {
        return value switch
        {
            >= 'A' and <= 'Z' => new[] { (byte)(value - 'A') },
            >= 'a' and <= 'z' => new[] { (byte)(value - 'a' + 26) },
            >= '0' and <= '9' => new[] { (byte)(value - '0' + 52) },
            '$' => FullAlphabet,
            _ => Array.Empty<byte>()
        };
    }

    public byte AlphabetSize => 62;
    public Regex AlphabetRegex => StaticAlphabetRegex;
    private static readonly Regex StaticAlphabetRegex = new Regex("^[A-Za-z0-9]$");
}