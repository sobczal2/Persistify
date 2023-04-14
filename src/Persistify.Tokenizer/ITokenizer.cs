using System.Reflection.Metadata;

namespace Persistify.Tokenizer;

public interface ITokenizer
{
    string[] Tokenize(string query);
}