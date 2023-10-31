using Persistify.Dtos.Common;

namespace Persistify.Client.HighLevel.Attributes;

public class PersistifyDateTimeFieldAttribute : PersistifyFieldAttribute
{
    public PersistifyDateTimeFieldAttribute(string? name = null, bool required = true) : base(name, required)
    {
    }

    public override FieldTypeDto FieldTypeDto => FieldTypeDto.DateTime;
}
