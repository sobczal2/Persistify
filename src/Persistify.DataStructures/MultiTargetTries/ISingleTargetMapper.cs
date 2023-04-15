using System.Text.RegularExpressions;

namespace Persistify.DataStructures.MultiTargetTries;

public interface ISingleTargetMapper
{
    byte AlphabetSize { get; }
    Regex AlphabetRegex { get; }
    byte MapToIndex(char value);
}