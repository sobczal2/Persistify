using Persistify.Indexer.Types;

namespace Persistify.Indexer.Tokens;

public interface ITokenizer
{
    Token[] Tokenize(TypeDefinition typeDefinition, string data);
}