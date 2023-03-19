namespace Persistify.Core.Tokens;

public class PlainTextTokenizer : ITokenizer
{
    public string[] Tokenize(string text)
    {
        return text.Split(' ');
    }
}