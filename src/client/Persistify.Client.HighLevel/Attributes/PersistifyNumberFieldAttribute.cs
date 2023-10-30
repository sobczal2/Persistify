using Persistify.Dtos.Common;

namespace Persistify.Client.HighLevel.Attributes;

public class PersistifyNumberFieldAttribute : PersistifyFieldAttribute
{
    public PersistifyNumberFieldAttribute(string? name = null, bool required = true)
        : base(name, required) { }

    public override FieldTypeDto FieldTypeDto => FieldTypeDto.Number;
}
