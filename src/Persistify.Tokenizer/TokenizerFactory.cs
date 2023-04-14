namespace Persistify.Tokenizer;

public class TokenizerFactory
{
    public ITokenizer CreateTokenizer(bool caseSensitive)
    {
        return caseSensitive ? new CaseSensitiveTokenizer() : new CaseInsensitiveTokenizer();
    }
}