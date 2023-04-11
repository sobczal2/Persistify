namespace Persistify.Dtos.Common.Types;

public class FieldDefinitionDto
{
    public string Path { get; init; } = default!;
    public FieldTypeDto Type { get; init; }
    public bool IsRequired { get; init; }
}