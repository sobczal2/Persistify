namespace Persistify.Server.Fts.Tokens;

public class SearchToken : Token
{
    public SearchToken(
        string value,
        char[] alphabet
    )
        : base(value, alphabet)
    {
    }
}
