using System;
using Persistify.Dtos.Common;

namespace Persistify.Client.HighLevel.Converters.Number;

public class IntToNumberPersistifyConverter : IPersistifyConverter
{
    public Type FromType => typeof(int);
    public FieldTypeDto FieldTypeDto => FieldTypeDto.Number;

    public object Convert(
        object from
    )
    {
        var fromInt = (int)from;
        return (double)fromInt;
    }

    public object ConvertBack(
        object to
    )
    {
        var toDouble = (double)to;
        return (int)toDouble;
    }
}
