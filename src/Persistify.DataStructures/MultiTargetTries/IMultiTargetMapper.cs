using System.Text.RegularExpressions;

namespace Persistify.DataStructures.MultiTargetTries;

public interface IMultiTargetMapper
{
    byte AlphabetSize { get; }
    Regex AlphabetRegex { get; }
    byte[] MapToIndexes(char value);
}