namespace Persistify.Fts.Analysis.Exceptions;

public class UnsupportedCharacterFilterException : Exception
{
    public UnsupportedCharacterFilterException(string characterFilterName,
        IEnumerable<string> supportedCharacterFilterNames)
        : base(
            $"Unsupported character filter: {characterFilterName}. Supported character filters: {string.Join(", ", supportedCharacterFilterNames)}")
    {
    }
}
