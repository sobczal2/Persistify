using Persistify.Dtos.Common;

namespace Persistify.Client.HighLevel.Attributes;

public class PersistifyBoolFieldAttribute : PersistifyFieldAttribute
{
    public PersistifyBoolFieldAttribute(string? name = null, bool required = true)
        : base(name, required) { }

    public override FieldTypeDto FieldTypeDto => FieldTypeDto.Bool;
}
