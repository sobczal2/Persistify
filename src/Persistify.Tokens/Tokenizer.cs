using System.Collections.Generic;

namespace Persistify.Tokens;

public class Tokenizer : ITokenizer
{
    public IEnumerable<Token<string>> TokenizeText(string text, string path)
    {
        var strings = text.ToLower().Split(' ');
        foreach (var str in strings)
            yield return new Token<string>(str, path);
    }

    public Token<double> TokenizeNumber(double number, string path)
    {
        return new Token<double>(number, path);
    }

    public Token<bool> TokenizeBoolean(bool boolean, string path)
    {
        return new Token<bool>(boolean, path);
    }

    public IEnumerable<Token<string>> TokenizeText(string text)
    {
        return TokenizeText(text, string.Empty);
    }
}