using System.Text.RegularExpressions;

namespace Persistify.DataStructures.MultiTargetTries;

public interface ISingleTargetMapper
{
    byte MapToIndex(char value);
    byte AlphabetSize { get; }
    Regex AlphabetRegex { get; }
}