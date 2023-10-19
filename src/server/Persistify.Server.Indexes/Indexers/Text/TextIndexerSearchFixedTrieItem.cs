using Persistify.Server.Fts.Tokens;
using Persistify.Server.Indexes.DataStructures.Tries;

namespace Persistify.Server.Indexes.Indexers.Text;

public class TextIndexerSearchFixedTrieItem : SearchFixedTrieItem
{
    private readonly Token _token;

    public TextIndexerSearchFixedTrieItem(
        Token token
        )
    {
        _token = token;
    }
    public override int GetIndex(int index)
    {
        return _token.AlphabetIndexMap[index];
    }

    public override int Length => _token.Value.Length;
    public override int AnyIndex => '?';
    public override int RepeatedAnyIndex => '*';
}
