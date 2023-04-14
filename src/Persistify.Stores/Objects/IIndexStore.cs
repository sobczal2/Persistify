using System.Reflection.Metadata;
using Persistify.Dtos.Common;
using Persistify.Stores.Common;
using OneOf;

namespace Persistify.Stores.Objects;

public interface IIndexStore
{
    public OneOf<StoreSuccess<EmptyDto>, StoreError> AddTokens(string typeName, string[] tokens, long documentId);
}