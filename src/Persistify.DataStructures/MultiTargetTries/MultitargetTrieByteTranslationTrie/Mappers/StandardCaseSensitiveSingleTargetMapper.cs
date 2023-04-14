using System.Text.RegularExpressions;
using Persistify.DataStructures.MultiTargetTries.Exceptions;

namespace Persistify.DataStructures.MultiTargetTries.MultitargetTrieByteTranslationTrie.Mappers;

public class StandardCaseSensitiveSingleTargetMapper : ISingleTargetMapper
{
    public byte MapToIndex(char value)
    {
        return value switch
        {
            >= 'A' and <= 'Z' => (byte)(value - 'A'),
            >= 'a' and <= 'z' => (byte)(value - 'a' + 26),
            >= '0' and <= '9' => (byte)(value - '0' + 52),
            _ => throw new AlphabetTooLargeException()
        };
    }

    public byte AlphabetSize => 62;
    public Regex AlphabetRegex => StaticAlphabetRegex;
    private static readonly Regex StaticAlphabetRegex = new Regex("^[A-Za-z0-9]$");
}