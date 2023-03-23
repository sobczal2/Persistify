using System.Threading.Tasks;
using Persistify.Indexer.Types;

namespace Persistify.Indexer;

public interface ITypeManager
{
    Task<bool> InitTypeAsync(TypeDefinition typeDefinition);
    Task<TypeDefinition[]> ListTypesAsync();
    Task<bool> DropTypeAsync(string typeName);
}