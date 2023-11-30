using Persistify.Dtos.Common;

namespace Persistify.Client.HighLevel.Attributes;

public class PersistifyBinaryFieldAttribute : PersistifyFieldAttribute
{
    public PersistifyBinaryFieldAttribute(
        string? name = null,
        bool required = true
    )
        : base(name, required)
    {
    }

    public override FieldTypeDto FieldTypeDto => FieldTypeDto.Binary;
}
