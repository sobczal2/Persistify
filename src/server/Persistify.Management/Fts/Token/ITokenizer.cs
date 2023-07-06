namespace Persistify.Management.Fts.Token;

public interface ITokenizer
{
    (string Term, float TermFrequency)[] Tokenize(string value);
    string[] TokenizeWithWildcards(string value);
}
