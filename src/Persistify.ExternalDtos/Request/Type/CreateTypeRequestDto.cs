using Persistify.ExternalDtos.Common.Types;

namespace Persistify.ExternalDtos.Request.Type;

public class CreateTypeRequestDto
{
    public TypeDefinitionDto TypeDefinition { get; init; } = default!;
}