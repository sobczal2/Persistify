using System.Text.RegularExpressions;

namespace Persistify.DataStructures.MultiTargetTries;

public interface IMultiTargetMapper
{
    byte[] MapToIndexes(char value);
    byte AlphabetSize { get; }
    Regex AlphabetRegex { get; }
}