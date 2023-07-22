namespace Persistify.Fts.Analysis.Exceptions;

public class UnsupportedTokenFilterException : Exception
{
    public UnsupportedTokenFilterException(string tokenFilterName, IEnumerable<string> supportedTokenFilterNames)
        : base(
            $"Unsupported token filter: {tokenFilterName}. Supported token filters: {string.Join(", ", supportedTokenFilterNames)}")
    {
    }
}
