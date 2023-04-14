namespace Persistify.Tokenizer;

public class CaseInsensitiveTokenizer : ITokenizer
{
    public string[] Tokenize(string query)
    {
        return query.ToLower().Split(' ');
    }
}