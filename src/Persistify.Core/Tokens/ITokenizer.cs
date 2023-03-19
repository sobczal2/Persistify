namespace Persistify.Core.Tokens;

public interface ITokenizer
{
    string[] Tokenize(string text);
}