using System.Threading.Tasks;
using Persistify.Indexer.Tokens;

namespace Persistify.Indexer.Index;

public interface IIndexStore
{
    void Add(string typeName, Token token, long id);
    long[] Search(string typeName, string query);
    bool Remove(string typeName, long id);
    void Clear(string typeName);
    long Count(string typeName);
}