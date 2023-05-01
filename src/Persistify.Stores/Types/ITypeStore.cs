using Persistify.Protos;

namespace Persistify.Stores.Types;

public interface ITypeStore
{
    bool Exists(string typeName);
    void Create(TypeDefinitionProto typeDefinitionDtoName);
    void Delete(string typeName);
    TypeDefinitionProto Get(string typeName);

    (TypeDefinitionProto[] TypeDefinitions, PaginationResponseProto PaginationResponse) List(
        PaginationRequestProto paginationRequest);
}