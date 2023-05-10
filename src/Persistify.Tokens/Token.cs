namespace Persistify.Tokens;

public struct Token<TValue>
    where TValue : notnull
{
    public TValue Value { get; set; }
    public string Path { get; set; }

    public Token(TValue value, string path)
    {
        Value = value;
        Path = path;
    }
}