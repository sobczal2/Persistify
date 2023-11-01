using System;
using Persistify.Dtos.Common;

namespace Persistify.Client.HighLevel.Converters.DateTime;

public class DateTimeToDateTimePersistifyConverter : IPersistifyConverter
{
    public Type FromType => typeof(System.DateTime);
    public FieldTypeDto FieldTypeDto => FieldTypeDto.DateTime;
    public object Convert(object from)
    {
        return from;
    }

    public object ConvertBack(object to)
    {
        return to;
    }
}
