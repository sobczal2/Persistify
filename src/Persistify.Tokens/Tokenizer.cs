using System.Collections.Generic;
using System.Text;

namespace Persistify.Tokens;

public class Tokenizer : ITokenizer
{
    public IEnumerable<Token<string>> TokenizeText(string text, string path)
    {
        var stringBuilder = new StringBuilder();
        var previousCharValid = false;

        for (var i = 0; i < text.Length; i++)
        {
            var currentChar = text[i];

            if (char.IsLetterOrDigit(currentChar))
            {
                stringBuilder.Append(currentChar);
                previousCharValid = true;
            }
            else if (previousCharValid)
            {
                yield return new Token<string>(stringBuilder.ToString(), path);
                stringBuilder.Clear();
                previousCharValid = false;
            }
        }

        if (previousCharValid)
        {
            yield return new Token<string>(stringBuilder.ToString(), path);
        }
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