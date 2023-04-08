namespace Persistify.ExternalDtos.Common.Types;

public class TypeDefinitionDto
{
    public string TypeName { get; init; } = default!;
    public FieldDefinitionDto[] Fields { get; init; } = default!;
    public string IdFieldPath { get; init; } = default!;
}