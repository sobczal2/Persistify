using Persistify.Dtos.Common.Types;

namespace Persistify.Dtos.Request.Type;

public class CreateTypeRequestDto
{
    public TypeDefinitionDto TypeDefinition { get; init; } = default!;
}