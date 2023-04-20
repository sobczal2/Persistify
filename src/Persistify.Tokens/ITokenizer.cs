using System.Collections.Generic;

namespace Persistify.Tokens;

public interface ITokenizer
{
    IEnumerable<Token<string>> TokenizeText(string text, string path);
    Token<double> TokenizeNumber(double number, string path);
    Token<bool> TokenizeBoolean(bool boolean, string path);
    IEnumerable<Token<string>> TokenizeText(string text);
}