using System.Threading.Tasks;
using Persistify.Indexer.Types;

namespace Persistify.Indexer.Core;

public interface IPersistifyManager
{
    Task<bool> InitTypeAsync(TypeDefinition typeDefinition);
    Task<bool> DropTypeAsync(string name);
    Task<TypeDefinition[]> ListTypesAsync();
    
    Task<long> IndexAsync(string type, string data);
    Task<Document[]> SearchAsync(string type, string query, int limit = 10, int offset = 0);
    Task DeleteAsync(string typeName, long id);
}