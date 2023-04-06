namespace Persistify.Indexer.Tokens;

public struct Token
{
    public Token(string value)
    {
        Value = value;
    }
    public string Value { get; set; }
}