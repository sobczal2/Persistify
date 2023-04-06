using System.Threading.Tasks;

namespace Persistify.Indexer.Types;

public interface ITypeStore
{
    Task<bool> InitTypeAsync(TypeDefinition typeDefinition);
    Task<TypeDefinition[]> ListTypesAsync();
    Task<bool> DropTypeAsync(string typeName);
    Task<TypeDefinition> GetTypeAsync(string typeName);
}