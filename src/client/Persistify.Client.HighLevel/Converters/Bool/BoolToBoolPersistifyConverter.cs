using System;
using Persistify.Dtos.Common;

namespace Persistify.Client.HighLevel.Converters.Bool;

public class BoolToBoolPersistifyConverter : IPersistifyConverter
{
    public Type FromType => typeof(bool);
    public FieldTypeDto FieldTypeDto => FieldTypeDto.Bool;
    public object Convert(object from)
    {
        return from;
    }

    public object ConvertBack(object to)
    {
        return to;
    }
}
