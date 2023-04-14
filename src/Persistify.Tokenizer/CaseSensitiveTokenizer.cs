namespace Persistify.Tokenizer;

public class CaseSensitiveTokenizer : ITokenizer
{
    public string[] Tokenize(string query)
    {
        return query.Split(' ');
    }
}